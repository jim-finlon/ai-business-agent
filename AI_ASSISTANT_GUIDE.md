# Guide: Working with Multiple AI Assistants

**Version:** 1.0  
**Date:** October 26, 2025

This guide explains how to effectively use the provided documentation to build the AI Agent Business Management System with multiple AI assistants.

---

## Document Overview

You now have three key documents:

1. **SYSTEM_REQUIREMENTS.md** - Complete technical specification
2. **TASK_BREAKDOWN.md** - Work packages for distribution
3. **This guide** - How to coordinate the work

---

## Recommended Workflow

### Phase 1: Setup & Foundation

**Week 1: Infrastructure Foundation**

**AI Assistant Alpha - Database Setup**
```
Prompt:
"I'm building an AI agent business management system. Please read the file 
SYSTEM_REQUIREMENTS.md and TASK_BREAKDOWN.md. Then complete 'Task Package 1: 
Database Infrastructure Setup'. Create all necessary Docker Compose files, 
database schemas, and seed data as specified."
```

**AI Assistant Beta - Authentication Service**
```
Prompt (after Package 1 is done):
"I'm building an AI agent business management system. Please read 
SYSTEM_REQUIREMENTS.md and TASK_BREAKDOWN.md. Complete 'Task Package 2: 
Authentication Service'. The database infrastructure is ready. Create the full 
authentication service with JWT support, user management, and all required tests."
```

### Phase 2: Core Services (Parallel)

**AI Assistant Gamma - Task Management**
```
Prompt:
"Please read SYSTEM_REQUIREMENTS.md and TASK_BREAKDOWN.md. Complete 
'Task Package 3: Task Management Service'. Authentication service is available 
at http://localhost:5001. Create the complete task management service with ABC 
prioritization, dependencies, and attachments."
```

**AI Assistant Delta - Notes Service**
```
Prompt:
"Please read SYSTEM_REQUIREMENTS.md and TASK_BREAKDOWN.md. Complete 
'Task Package 4: Notes Service'. Create the notes service with MongoDB, rich 
content support, and full-text search."
```

**AI Assistant Zeta - File Storage**
```
Prompt:
"Please read SYSTEM_REQUIREMENTS.md and TASK_BREAKDOWN.md. Complete 
'Task Package 6: File Storage Service'. Create the storage service with MinIO 
integration, virus scanning, and quota management."
```

### Phase 3: Advanced Services

**AI Assistant Epsilon - Agent Queue**
```
Prompt:
"Please read SYSTEM_REQUIREMENTS.md and TASK_BREAKDOWN.md. Complete 
'Task Package 5: Agent Queue Service'. Task Management, Notes, and Storage 
services are ready. Create the queue service with RabbitMQ, background workers, 
and progress tracking."
```

**AI Assistant Eta - Notifications**
```
Prompt:
"Please read SYSTEM_REQUIREMENTS.md and TASK_BREAKDOWN.md. Complete 
'Task Package 7: Notification Service'. Create the notification service with 
multi-channel support (email, push, webhooks) and templates."
```

### Phase 4: Integration

**AI Assistant Theta - API Gateway**
```
Prompt:
"Please read SYSTEM_REQUIREMENTS.md and TASK_BREAKDOWN.md. Complete 
'Task Package 8: API Gateway'. All backend services are ready. Create the API 
gateway with YARP, authentication, rate limiting, and health checks."
```

**AI Assistant Iota - MCP Servers**
```
Prompt:
"Please read SYSTEM_REQUIREMENTS.md and TASK_BREAKDOWN.md. Complete 
'Task Package 9: MCP Server Implementation'. All services are ready. Implement 
MCP servers for Task Management, Notes, and Agent Queue services following the 
Model Context Protocol specification."
```

### Phase 5: Mobile & Infrastructure

**AI Assistant Kappa - Mobile App**
```
Prompt:
"Please read SYSTEM_REQUIREMENTS.md and TASK_BREAKDOWN.md. Complete 
'Task Package 10: Mobile Application'. All backend services and APIs are ready. 
Create the .NET MAUI Android app with task management, notes, agent queue, and 
offline support."
```

**AI Assistant Lambda - Terraform**
```
Prompt:
"Please read SYSTEM_REQUIREMENTS.md and TASK_BREAKDOWN.md. Complete 
'Task Package 11: Infrastructure as Code'. Create Terraform modules for AWS to 
provision VPC, EKS cluster, RDS, DocumentDB, ElastiCache, and S3."
```

**AI Assistant Mu - Helm Charts**
```
Prompt:
"Please read SYSTEM_REQUIREMENTS.md and TASK_BREAKDOWN.md. Complete 
'Task Package 12: Kubernetes Helm Charts'. Create Helm charts for all services 
with proper configurations, secrets, HPA, and ingress rules."
```

---

## Best Practices for AI Prompting

### 1. Always Provide Context

✅ Good:
```
"I'm building a microservices-based business management system. Please read 
SYSTEM_REQUIREMENTS.md for full context. I need you to implement the Task 
Management Service as described in Task Package 3 of TASK_BREAKDOWN.md."
```

❌ Bad:
```
"Make a task management API"
```

### 2. Reference Documentation

✅ Good:
```
"Please implement the database schema exactly as specified in 
SYSTEM_REQUIREMENTS.md section 'Task Management Service - Database Schema'. Use 
PostgreSQL with the indexes and constraints shown."
```

❌ Bad:
```
"Create a database for tasks"
```

### 3. Specify Technologies

✅ Good:
```
"Use ASP.NET Core 9 Minimal APIs with Carter, Entity Framework Core 9, MediatR 
for CQRS, FluentValidation for input validation, and xUnit for testing - as 
specified in the technology stack."
```

❌ Bad:
```
"Use modern .NET technologies"
```

### 4. Request Tests

✅ Good:
```
"Implement the authentication service with unit tests using xUnit, Moq, and 
FluentAssertions. Also create integration tests using WebApplicationFactory and 
Testcontainers. Aim for 80%+ code coverage."
```

❌ Bad:
```
"Add some tests"
```

### 5. Ask for Documentation

✅ Good:
```
"After implementing the API, generate OpenAPI/Swagger documentation. Also create 
a README.md with setup instructions, example API calls, and troubleshooting tips."
```

❌ Bad:
```
"Document the API"
```

### 6. Iterative Refinement

First prompt:
```
"Implement the core Task CRUD operations from Task Package 3"
```

Follow-up:
```
"Great! Now add the ABC prioritization logic with automatic ordering within 
priority groups. Ensure tasks can be reordered via drag-and-drop API."
```

Next:
```
"Now implement the dependency management. Tasks can depend on other tasks, and 
we need to validate no circular dependencies exist. Also prevent completing a 
task if its dependencies are incomplete."
```

---

## Coordination Strategies

### Strategy 1: Sequential Dependencies

When tasks have clear dependencies, work sequentially:

1. Database Infrastructure → Auth Service → All other services
2. Backend Services → API Gateway → Mobile App
3. Services → Helm Charts → CI/CD

### Strategy 2: Parallel Work

When tasks are independent, work in parallel:

- Task Management + Notes + Storage (all can run simultaneously)
- Terraform + Mobile App development
- Documentation + E2E tests

### Strategy 3: Hybrid Approach

Most efficient for this project:

**Week 1-2:**
- Sequential: Database → Auth (foundational)

**Week 3-6:**
- Parallel: Task Mgmt + Notes + Storage + Terraform
- Each AI assistant works independently

**Week 7-10:**
- Parallel: Agent Queue + Notifications + API Gateway + Mobile
- Some coordination needed (API contracts)

**Week 11-14:**
- Parallel: MCP Servers + Helm Charts + CI/CD
- Sequential: Deploy to staging → E2E tests

---

## Handling AI Assistant Limitations

### Token Limits

**Problem:** Large files may exceed context windows

**Solution:**
```
"Please implement Task Package 3, but focus only on the core CRUD operations 
first. We'll add advanced features (prioritization, dependencies) in subsequent 
conversations."
```

### File Handling

**Problem:** AI might not handle large file uploads well

**Solution:**
```
"Generate the code but don't try to upload files directly. Instead, create a 
scripts/setup.sh that I can run to set up the project structure."
```

### Testing Coverage

**Problem:** AI might skip tests or create incomplete tests

**Solution:**
```
"After implementing the service, create comprehensive tests. For each endpoint, 
write: 1) Happy path test, 2) Invalid input test, 3) Unauthorized access test, 
4) Not found test. Show me the test files."
```

---

## Verification Checklist

After each AI assistant completes a task package:

### Code Quality
- [ ] Code follows C# conventions (PascalCase, etc.)
- [ ] No hardcoded secrets or passwords
- [ ] Proper error handling (try-catch, custom exceptions)
- [ ] Logging statements in appropriate places
- [ ] XML documentation on public APIs

### Testing
- [ ] Unit tests exist and pass
- [ ] Integration tests exist and pass
- [ ] Test coverage meets targets (check with `dotnet test /p:CollectCoverage=true`)
- [ ] Edge cases covered

### Configuration
- [ ] appsettings.json has all required settings
- [ ] appsettings.Development.json for dev overrides
- [ ] Environment variables documented
- [ ] Secrets not in code (use user secrets for dev)

### Documentation
- [ ] README.md exists in project
- [ ] API endpoints documented (Swagger/OpenAPI)
- [ ] Setup instructions provided
- [ ] Example requests/responses shown

### Docker
- [ ] Dockerfile exists and builds successfully
- [ ] Multi-stage build for smaller images
- [ ] Non-root user in container
- [ ] Health check defined

---

## Integration Testing Between Services

After completing multiple services:

```
Prompt to Integration AI Assistant:
"I have the following services running:
- Auth Service: http://localhost:5001
- Task Management: http://localhost:5002
- Notes Service: http://localhost:5003

Please create integration tests that:
1. Register a user via Auth Service
2. Create a task via Task Management (using token from step 1)
3. Create a note via Notes Service (using same token)
4. Create a task from the note (cross-service integration)

Use xUnit and Refit for the API clients. Run all services in Docker Compose 
for testing."
```

---

## Common Issues & Solutions

### Issue: Services can't connect to each other

**Solution:**
```
Check docker-compose.yml:
- All services on same network?
- Using service names (not localhost) for inter-service calls?
- Ports exposed correctly?
```

### Issue: Database migrations fail

**Solution:**
```
1. Check connection string is correct
2. Ensure database container is healthy before migration
3. Use wait-for-it.sh script in Docker entrypoint
```

### Issue: Tests fail in CI but pass locally

**Solution:**
```
1. Check if tests depend on local environment (paths, ports)
2. Use Testcontainers to ensure consistent environment
3. Add more logging to CI
```

### Issue: AI generates code that doesn't compile

**Solution:**
```
"The code you generated has compilation errors. Please fix:
[paste error messages]

Ensure you're using .NET 9 syntax and the specified package versions."
```

### Issue: AI forgets context from earlier in conversation

**Solution:**
```
"As a reminder, we're working on Task Package 3: Task Management Service. 
Previously, you implemented the CRUD operations. Now we need to add the ABC 
prioritization feature. Please refer back to SYSTEM_REQUIREMENTS.md for the 
exact specification."
```

---

## Sample AI Assistant Assignments

Here's a suggested assignment of tasks to specific AI tools:

### Claude (via Claude.ai or API)
- **Best for:** Complex logic, architecture decisions, CQRS implementations
- **Assign:** Task Management Service, Agent Queue Service, API Gateway

### GPT-4 (via ChatGPT or API)
- **Best for:** Well-documented patterns, standard CRUD operations
- **Assign:** Auth Service, Notes Service, Storage Service

### GitHub Copilot
- **Best for:** Completing code, writing tests, boilerplate
- **Use alongside:** Manual coding of complex features

### Cursor
- **Best for:** Editing existing codebases, refactoring
- **Assign:** Code reviews, optimization, adding tests to existing code

### Specialized AI (if using)
- **Tabnine, CodeWhisperer:** Similar to Copilot, use for completion
- **Mintlify:** Documentation generation
- **Snyk AI:** Security fixes

---

## Tracking Progress

### Use a Project Board

Create columns:
- **Backlog:** All 15 task packages
- **In Progress:** Currently being worked on
- **Review:** Completed, needs verification
- **Done:** Verified and integrated

### Status Updates

Create a `STATUS.md` file:

```markdown
# Project Status - Updated: 2025-10-26

## Completed
- ✅ Task Package 1: Database Infrastructure
- ✅ Task Package 2: Authentication Service

## In Progress
- 🔄 Task Package 3: Task Management (AI: Claude, 60% done)
- 🔄 Task Package 4: Notes Service (AI: GPT-4, 40% done)

## Blocked
- ⛔ Task Package 5: Agent Queue (waiting on Task Package 3)

## Not Started
- ⏸️ Task Package 6: File Storage
- ⏸️ Task Package 7: Notifications
... (remaining packages)
```

---

## Final Integration Prompt

After all services are complete:

```
Prompt:
"All services are now implemented. Please help me with final integration:

1. Review docker-compose.yml to ensure all services are configured correctly
2. Create a master docker-compose.yml that brings up entire system
3. Create an integration test suite that tests critical user journeys:
   - User registration and login
   - Creating a task with attachments
   - Creating a note and converting it to a task
   - Queueing an AI agent task and checking results
4. Create a smoke test script for production deployment
5. Document the startup sequence and verification steps

All services and documentation are in the project repository. Let me know if 
you need to see specific files."
```

---

## Troubleshooting AI Assistants

### AI is generating outdated code

```
"Please use .NET 9 with the latest C# 12 syntax. For example:
- Use primary constructors
- Use collection expressions
- Use required properties
- Use file-scoped namespaces

Here's an example of the style I want:
[paste example code from SYSTEM_REQUIREMENTS.md]"
```

### AI is not following architecture

```
"This code doesn't follow Clean Architecture as specified. Please reorganize:

Core layer should contain:
- Domain entities (no dependencies)
- Application layer (commands, queries, interfaces)

Infrastructure layer should contain:
- EF Core implementations
- External service integrations
- Repository implementations

API layer should contain:
- Controllers/endpoints
- Middleware
- DTOs for requests/responses

Please refactor accordingly."
```

### AI is hallucinating features

```
"Please stick strictly to the requirements in SYSTEM_REQUIREMENTS.md. Don't add 
features not specified. If you think a feature would be beneficial, suggest it 
separately but don't implement it without confirmation."
```

---

## Success Criteria

At the end of the project, you should have:

✅ All 15 task packages completed  
✅ Every service with 80%+ test coverage  
✅ All services running in Docker Compose  
✅ Swagger documentation for all APIs  
✅ Mobile app builds and runs  
✅ Helm charts deploy to Kubernetes successfully  
✅ CI/CD pipeline deploys automatically  
✅ Monitoring dashboards show metrics  
✅ All documentation complete  

---

## Tips for Efficiency

1. **Start Simple:** Get a minimal version working first, then add features
2. **Test Often:** Don't wait until everything is built to test integration
3. **Use Templates:** Once one service is built well, use it as a template
4. **Save Good Prompts:** When you find prompts that work well, save them
5. **Iterate:** Don't expect perfection on first try, refine iteratively
6. **Document as You Go:** Don't leave all documentation for the end

---

## Estimated Timeline with AI Assistants

**With 3-4 AI assistants working in parallel:**

- **Week 1-2:** Foundation (Packages 1-2)
- **Week 3-6:** Core Services (Packages 3-6)  
- **Week 7-10:** Advanced Services (Packages 5, 7-9)
- **Week 11-14:** Mobile & Infrastructure (Packages 10-12)
- **Week 15-18:** DevOps & Testing (Packages 13-15)
- **Week 19-20:** Integration, polish, deployment

**Total: ~20 weeks (5 months)**

Note: This assumes you're reviewing and integrating the AI-generated code, not 
just accepting it blindly. Factor in time for:
- Reviewing generated code
- Testing integrations
- Fixing bugs
- Refactoring for quality
- Addressing security issues

---

## Next Steps

1. **Review Documents:** Read SYSTEM_REQUIREMENTS.md and TASK_BREAKDOWN.md thoroughly
2. **Set Up Repository:** Create GitHub repo with proper structure
3. **Start with Package 1:** Get databases running in Docker
4. **Proceed Sequentially:** Follow the dependency graph
5. **Track Progress:** Update STATUS.md regularly
6. **Iterate and Refine:** Don't expect perfection, iterate towards quality

---

**Good luck building your AI Agent Business Management System!**

---

**Document End**
