import json


class WordRequest:
    def __init__(self, word: str):
        self.word = word

    def json(self):
        return {
            "word": self.word
        }

class WordDefinitionResponse:
    partOfSpeech: str
    ipa: str
    definition: str
    cerfLevel: str
    synonyms: list[str]
    antonyms: list[str]

    @staticmethod
    def fromJSON(json: dict) -> "WordDefinitionResponse":
        instance = WordDefinitionResponse()
        instance.partOfSpeech = json["partOfSpeech"]
        instance.ipa = json["ipa"]
        instance.definition = json["definition"]
        instance.cerfLevel = json["cerfLevel"]
        instance.synonyms = json["synonyms"]
        instance.antonyms = json["antonyms"]
        return instance

    def toObject(self):
        return {
            "partOfSpeech": self.partOfSpeech,
            "ipa": self.ipa,
            "definition": self.definition,
            "cerfLevel": self.cerfLevel,
            "synonyms": self.synonyms,
            "antonyms": self.antonyms
        }

class WordResponse:
    word: str
    definitions: list[WordDefinitionResponse]

    def __init__(self, word: str, definitions: list[WordDefinitionResponse]):
        self.word = word
        self.definitions = definitions

    def toObject(self):
        return {
            "word": self.word,
            "definitions": [defn.toObject() for defn in self.definitions]
        }

    def json(self):
        return json.dumps(self.toObject())
    def __str__(self):
        return self.json()
