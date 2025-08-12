# GitHub Workflows

This directory contains GitHub Actions workflows for the Dictionary Generator project. These workflows focus on **code quality checking and validation** without any deployment or push operations. Below is an overview of each workflow:

## 🔧 Core Workflows

### [`build.yml`](./build.yml)
**Code Quality Check workflow** - Runs on every push and pull request to main/develop branches.

- **Jobs:**
  - `build-dotnet`: Builds .NET services, runs tests, verifies code formatting
  - `build-python`: Builds Python AI service, runs linting, security checks, and tests  
  - `build-docker`: Builds Docker images for verification (no push)
  - `integration-test`: Runs local integration tests with Docker Compose
  - `security-scan`: Scans for vulnerabilities using Trivy

- **Triggers:** Push/PR to main or develop branches
- **Duration:** ~10-15 minutes
- **Purpose:** Verify code quality, run tests, check security - **no deployments**

### [`pr-checks.yml`](./pr-checks.yml)
**Pull request quality checks** - Comprehensive checks for pull requests.

- **Jobs:**
  - `changes`: Detects which parts of the codebase changed
  - `lint-dotnet`: .NET code formatting and analysis
  - `lint-python`: Python code linting (flake8, black, isort, mypy, bandit)
  - `test-coverage`: Runs tests with coverage reporting
  - `docker-security`: Security scanning of Docker images (local build only)
  - `dependency-review`: Reviews dependency changes
  - `pr-size-check`: Labels PRs by size
  - `pr-comment`: Posts summary comment on PR

- **Triggers:** PR opened/updated to main or develop branches
- **Purpose:** Ensure code quality before merging - **no deployments**

## 🤖 Automation Workflows

### [`auto-label.yml`](./auto-label.yml)
**Automatic labeling** - Adds relevant labels to issues and PRs based on content and file changes.

- **Issue Labels:** bug, enhancement, documentation, question, backend, ai, docker, rabbitmq
- **PR Labels:** Based on changed files (backend, ai, docker, docs, github-actions)
- **Triggers:** New issues and PRs

### [`dependabot.yml`](../dependabot.yml)
**Dependency updates** - Configures Dependabot to automatically update dependencies.

- **Package Ecosystems:**
  - NuGet (.NET packages)
  - pip (Python packages)  
  - Docker (base images)
  - GitHub Actions (workflow actions)
- **Schedule:** Weekly updates on different days
- **Auto-assigns:** @beohoang98

## 🚀 Quick Start

1. **First-time setup:**
   ```bash
   # The workflows will run automatically when you push code
   git add .github/
   git commit -m "Add GitHub workflows for code quality"
   git push
   ```

2. **All workflows focus on code quality:**
   - Build verification
   - Test execution
   - Code formatting checks
   - Security scanning
   - Dependency reviews

## 🔒 Security Features

- **Vulnerability Scanning:** Trivy scans for security issues in code and Docker images
- **Dependency Review:** Checks for vulnerable dependencies in PRs
- **Security Policies:** Bandit checks Python code for security issues
- **SARIF Upload:** Security results are uploaded to GitHub Security tab

## 📊 Monitoring & Reporting

- **Test Coverage:** Coverage reports uploaded to Codecov
- **Build Status:** Status badges available for README
- **PR Comments:** Automated quality check summaries
- **Security Reports:** Vulnerability findings in GitHub Security tab

## 🛠️ Customization

### Environment Variables
Add these secrets in your GitHub repository settings if needed:

```
# Optional: Add these for enhanced functionality
CODECOV_TOKEN          # For coverage reporting
GITHUB_TOKEN           # Automatically provided
```

### Modifying Workflows

1. **Change .NET version:** Update `DOTNET_VERSION` in workflow files
2. **Change Python version:** Update `PYTHON_VERSION` in workflow files  
3. **Add new checks:** Add jobs to `pr-checks.yml`
4. **Customize security scans:** Modify security scanning parameters

### Branch Protection Rules

Recommended branch protection rules for `main` branch:
- Require status checks: `build-dotnet`, `build-python`, `integration-test`
- Require PR reviews: 1 reviewer
- Dismiss stale reviews when new commits are pushed
- Require linear history

## 📝 Workflow Status

You can add status badges to your README:

```markdown
![Code Quality](https://github.com/beohoang98/demo-dotnet-rabbitmq/workflows/Code%20Quality%20Check/badge.svg)
![PR Checks](https://github.com/beohoang98/demo-dotnet-rabbitmq/workflows/Pull%20Request%20Checks/badge.svg)
```

## 🐛 Troubleshooting

**Common Issues:**

1. **Build failures:** Check that .NET 9.0 SDK is properly configured
2. **Docker build issues:** Ensure Dockerfiles are present and valid
3. **Test failures:** Make sure all tests pass locally before pushing
4. **Linting errors:** Run `dotnet format` and `black`/`isort` locally first

**Debug Tips:**
- Enable debug logging by setting `ACTIONS_STEP_DEBUG: true` in workflow
- Check workflow logs in the Actions tab
- Use `act` tool to run workflows locally for testing

## 🎯 What These Workflows Do

✅ **Build and compile** your .NET and Python code
✅ **Run all tests** to ensure functionality
✅ **Check code formatting** and style consistency
✅ **Scan for security vulnerabilities** in code and dependencies
✅ **Verify Docker images** can be built successfully
✅ **Run integration tests** locally with Docker Compose

❌ **Do NOT push** Docker images to any registry
❌ **Do NOT deploy** to any environment
❌ **Do NOT publish** artifacts to package managers

Perfect for development workflows where you want to ensure code quality without any deployment concerns!
