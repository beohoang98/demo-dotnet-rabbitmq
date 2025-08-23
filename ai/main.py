import json
import logging
import os
import time
import pika

from lib.generator import WordGenerator
from models.word import WordRequest

RABBITMQ_HOST = os.getenv("RABBITMQ_HOST", "localhost")
RABBITMQ_USER = os.getenv("RABBITMQ_USER", "user")
RABBITMQ_PASSWORD = os.getenv("RABBITMQ_PASSWORD", "password")

connParams = pika.ConnectionParameters(
    host=RABBITMQ_HOST,
    credentials=pika.PlainCredentials(RABBITMQ_USER, RABBITMQ_PASSWORD)
)
logging.basicConfig(
    level=logging.INFO, format="%(asctime)s - %(levelname)s - %(message)s"
)


def main():
    while True:
        try:
            connection = pika.BlockingConnection(connParams)
            channel = connection.channel()
            channel.queue_declare(queue="words", durable=True)
            channel.basic_consume(
                queue="words", on_message_callback=callback, auto_ack=False
            )
            print("Waiting for messages. To exit press CTRL+C")
            channel.start_consuming()
        except pika.exceptions.AMQPConnectionError as e:
            print(f"Connection failed: {e}, retrying...")
            time.sleep(5)
            continue
        except KeyboardInterrupt:
            print("Interrupted by user, shutting down...")
            channel.stop_consuming()
            connection.close()
            break
        except Exception as e:
            print(f"An error occurred: {e}, retrying...")
            time.sleep(5)
            continue


def callback(ch, method, properties, body):
    print(f"Received message: {body.decode()}")
    try:
        reqInJson = json.loads(body.decode())
        wordReq = WordRequest(**reqInJson)
        wordGen = WordGenerator(model_name="gpt-4o-mini")
        wordRes = wordGen.generate(wordReq)
        wordJson = wordRes.json()
        logging.info(f"Processed word: {wordReq.word}, response: {wordJson}")

        # publish the response to the "dictionary-word" queue
        ch.basic_publish(
            exchange="", routing_key="dictionary-word", body=wordJson
        )

        ch.basic_ack(delivery_tag=method.delivery_tag)

    except Exception as e:
        logging.error(f"Error processing message: {e}")


if __name__ == "__main__":
    print(f"Starting RabbitMQ consumer..., pid={os.getpid()}")
    main()
