# AI Agent Business Management System - Requirements Document

**Version:** 1.0  
**Date:** October 26, 2025  
**Document Owner:** Business Entrepreneur  
**Project Type:** Distributed AI-Powered Business Automation Platform

---

## Executive Summary

This project aims to build a comprehensive AI-agent-driven business management platform that enables entrepreneurs to delegate routine tasks, manage priorities, capture ideas, and automate workflows through intelligent agents. The system will be built as a collection of microservices using .NET C#, containerized for cloud-native deployment, and accessible through multiple interfaces including mobile applications.

### Key Objectives

1. **Task Automation**: Enable AI agents to execute business tasks autonomously
2. **Intelligent Prioritization**: ABC priority system with smart ranking
3. **Knowledge Capture**: Comprehensive notes and ideas management with rich media support
4. **Mobile-First Access**: Android application for on-the-go task queuing
5. **Extensibility**: Plugin architecture for future integrations (banking, messaging, IoT)
6. **Enterprise-Grade**: Security, compliance, testing, and infrastructure as code

---

## System Architecture Overview

### Architecture Style
- **Microservices Architecture**: Each major component is an independent service
- **API-First Design**: All services expose RESTful APIs and MCP (Model Context Protocol) servers
- **Event-Driven**: Services communicate via message queues for async operations
- **CQRS Pattern**: Command Query Responsibility Segregation where appropriate
- **Database per Service**: Each microservice owns its data

### Technology Stack

#### Core Technologies
- **Language**: C# 12+ (.NET 9+)
- **API Framework**: ASP.NET Core Minimal APIs with Carter
- **Authentication**: ASP.NET Core Identity with JWT + OAuth2
- **Mobile**: .NET MAUI (Android-first, iOS-compatible)
- **Real-time**: SignalR for live updates
- **Validation**: FluentValidation
- **Mapping**: Mapster or AutoMapper

#### Data Persistence
- **Relational DB**: PostgreSQL 16+ (primary)
- **Document DB**: MongoDB 7+ (for unstructured data, notes, attachments)
- **Cache**: Redis 7+ (distributed cache, session management)
- **Message Queue**: RabbitMQ or Azure Service Bus
- **Object Storage**: MinIO or Azure Blob Storage (for files/images)

#### Testing & Quality
- **Unit Tests**: xUnit + FluentAssertions + Moq
- **Integration Tests**: WebApplicationFactory + Testcontainers
- **E2E Tests**: Playwright for web, Appium for mobile
- **Coverage Target**: Minimum 80% code coverage
- **API Testing**: REST Client files (.http) for manual testing

#### Infrastructure & DevOps
- **Containerization**: Docker 24+
- **Orchestration**: Docker Compose (dev), Kubernetes 1.28+ (prod)
- **Package Management**: Helm 3+
- **IaC**: Terraform 1.6+ for cloud resources
- **CI/CD**: GitHub Actions or GitLab CI
- **Monitoring**: Prometheus + Grafana, Seq for logs
- **Tracing**: OpenTelemetry

#### Security & Compliance
- **Security Standard**: OWASP Top 10 compliance
- **Accessibility**: WCAG 2.1 Level AA compliance
- **Secrets Management**: HashiCorp Vault or Azure Key Vault
- **API Security**: Rate limiting, CORS, API keys
- **Data Protection**: Encryption at rest and in transit (TLS 1.3)

---

## Component Breakdown

### 1. Task Management Service (Core Service)

**Service Name**: `taskmanager-service`  
**Primary Database**: PostgreSQL  
**Cache**: Redis

#### Functional Requirements

##### FR-TM-001: Task CRUD Operations
- Create, Read, Update, Delete tasks
- Each task must have:
  - Unique ID (GUID)
  - Title (required, max 200 chars)
  - Description (optional, rich text, max 5000 chars)
  - Status (Not Started, In Progress, Completed, Blocked, Cancelled)
  - Priority (A, B, C)
  - Priority Order (integer within priority group, auto-increment)
  - Due Date (optional)
  - Created Date (auto)
  - Modified Date (auto)
  - Completed Date (nullable)
  - Estimated Duration (minutes)
  - Actual Duration (minutes, nullable)
  - Tags (array of strings)
  - Categories (array of strings)
  - Assigned To (user ID or agent ID)
  - Parent Task ID (for subtasks)
  - Recurrence Pattern (none, daily, weekly, monthly, custom CRON)

##### FR-TM-002: ABC Prioritization System
- **Priority A**: Critical/urgent tasks
- **Priority B**: Important but not urgent
- **Priority C**: Nice to have/low priority
- Each priority group has numbered ordering (1, 2, 3...)
- Drag-and-drop reordering within priority groups
- Automatic reordering when tasks are added/removed
- Smart suggestions for priority based on:
  - Due dates
  - Keywords in title/description
  - Historical patterns
  - Dependencies

##### FR-TM-003: Task Attachments
- Support multiple attachments per task
- Allowed file types: images (jpg, png, gif, webp), documents (pdf, docx, xlsx, txt, md), archives (zip)
- Maximum file size: 50MB per file
- Total attachments per task: 100MB
- Store in object storage with references in database
- Generate thumbnails for images
- OCR text extraction from images and PDFs for search

##### FR-TM-004: Task Reminders
- Multiple reminders per task
- Reminder types:
  - One-time (specific date/time)
  - Before due date (X minutes/hours/days before)
  - Recurring (for recurring tasks)
- Delivery channels:
  - In-app notification
  - Push notification (mobile)
  - Email
  - Webhook to AI agent
- Snooze functionality (5min, 15min, 1hr, custom)

##### FR-TM-005: Task Queries
- Get tasks by date (specific day)
- Get tasks by date range
- Filter by status, priority, tags, categories, assignee
- Search full-text across title, description, and attachment text
- Sort by: priority order, due date, created date, modified date
- Pagination (default 50 items per page)
- Export to CSV, JSON, Excel

##### FR-TM-006: Task Templates
- Save common task patterns as templates
- Templates include: title format, default priority, tags, categories, subtasks
- Apply template to create new task quickly
- Share templates across team (if multi-user)

##### FR-TM-007: Task Dependencies
- Mark tasks as dependent on other tasks
- Prevent marking task as complete if dependencies incomplete
- Visualize dependency chains
- Suggest optimal order based on dependencies

##### FR-TM-008: Smart Scheduling
- AI-powered scheduling suggestions
- Consider:
  - Task duration estimates
  - Priority
  - Dependencies
  - User's available time blocks
  - Historical completion patterns
- Auto-schedule unscheduled tasks
- Reschedule automatically when priorities change

#### Non-Functional Requirements

##### NFR-TM-001: Performance
- API response time: p95 < 200ms, p99 < 500ms
- Support 1000 tasks per user without degradation
- Search results: < 100ms for typical queries
- Attachment upload: streaming for large files, progress indicator

##### NFR-TM-002: Scalability
- Horizontal scaling of API servers
- Database connection pooling (min 10, max 100)
- Read replicas for heavy queries
- Cache frequently accessed tasks (TTL: 5 minutes)

##### NFR-TM-003: Reliability
- 99.9% uptime SLA
- Automatic retry for transient failures (exponential backoff)
- Circuit breaker pattern for external dependencies
- Database backups every 6 hours, retained for 30 days

##### NFR-TM-004: Security
- Role-based access control (RBAC)
- Users can only access their own tasks (or team tasks if applicable)
- API rate limiting: 1000 requests per user per hour
- Input validation and sanitization
- SQL injection prevention (parameterized queries)
- XSS prevention (encode outputs)

#### API Endpoints (RESTful)

```
POST   /api/v1/tasks                    - Create task
GET    /api/v1/tasks                    - List tasks (with filters)
GET    /api/v1/tasks/{id}               - Get task by ID
PUT    /api/v1/tasks/{id}               - Update task
DELETE /api/v1/tasks/{id}               - Delete task
PATCH  /api/v1/tasks/{id}/status        - Update status only
PATCH  /api/v1/tasks/{id}/priority      - Update priority
POST   /api/v1/tasks/{id}/attachments   - Upload attachment
DELETE /api/v1/tasks/{id}/attachments/{attachmentId} - Delete attachment
GET    /api/v1/tasks/by-date/{date}     - Get tasks for specific date
GET    /api/v1/tasks/by-range           - Get tasks in date range
POST   /api/v1/tasks/{id}/reminders     - Add reminder
PUT    /api/v1/tasks/{id}/reminders/{reminderId} - Update reminder
DELETE /api/v1/tasks/{id}/reminders/{reminderId} - Delete reminder
POST   /api/v1/tasks/{id}/subtasks      - Create subtask
GET    /api/v1/tasks/{id}/dependencies  - Get dependency graph
POST   /api/v1/tasks/reorder            - Reorder tasks within priority
POST   /api/v1/tasks/templates          - Create template
GET    /api/v1/tasks/templates          - List templates
POST   /api/v1/tasks/from-template/{templateId} - Create task from template
GET    /api/v1/tasks/search             - Full-text search
POST   /api/v1/tasks/auto-schedule      - Get scheduling suggestions
GET    /api/v1/tasks/export             - Export tasks (CSV/JSON/Excel)
```

#### MCP Server Integration

**MCP Server Name**: `task-management-mcp`

##### MCP Tools Exposed

```json
{
  "tools": [
    {
      "name": "create_task",
      "description": "Create a new task with specified properties",
      "inputSchema": {
        "type": "object",
        "properties": {
          "title": { "type": "string" },
          "description": { "type": "string" },
          "priority": { "type": "string", "enum": ["A", "B", "C"] },
          "dueDate": { "type": "string", "format": "date-time" },
          "tags": { "type": "array", "items": { "type": "string" } },
          "categories": { "type": "array", "items": { "type": "string" } }
        },
        "required": ["title"]
      }
    },
    {
      "name": "get_tasks",
      "description": "Retrieve tasks with optional filters",
      "inputSchema": {
        "type": "object",
        "properties": {
          "status": { "type": "string" },
          "priority": { "type": "string" },
          "date": { "type": "string", "format": "date" },
          "dateRange": {
            "type": "object",
            "properties": {
              "start": { "type": "string", "format": "date" },
              "end": { "type": "string", "format": "date" }
            }
          },
          "tags": { "type": "array", "items": { "type": "string" } },
          "limit": { "type": "integer", "default": 50 }
        }
      }
    },
    {
      "name": "update_task",
      "description": "Update an existing task",
      "inputSchema": {
        "type": "object",
        "properties": {
          "taskId": { "type": "string" },
          "updates": { "type": "object" }
        },
        "required": ["taskId", "updates"]
      }
    },
    {
      "name": "complete_task",
      "description": "Mark a task as completed",
      "inputSchema": {
        "type": "object",
        "properties": {
          "taskId": { "type": "string" }
        },
        "required": ["taskId"]
      }
    },
    {
      "name": "prioritize_tasks",
      "description": "Get AI suggestions for task prioritization",
      "inputSchema": {
        "type": "object",
        "properties": {
          "criteria": { "type": "string" }
        }
      }
    },
    {
      "name": "search_tasks",
      "description": "Full-text search across tasks",
      "inputSchema": {
        "type": "object",
        "properties": {
          "query": { "type": "string" },
          "filters": { "type": "object" }
        },
        "required": ["query"]
      }
    }
  ]
}
```

#### Database Schema (PostgreSQL)

```sql
-- Core task table
CREATE TABLE tasks (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    parent_task_id UUID REFERENCES tasks(id) ON DELETE CASCADE,
    title VARCHAR(200) NOT NULL,
    description TEXT,
    status VARCHAR(50) NOT NULL DEFAULT 'NotStarted',
    priority CHAR(1) NOT NULL CHECK (priority IN ('A', 'B', 'C')),
    priority_order INTEGER NOT NULL,
    due_date TIMESTAMP WITH TIME ZONE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    modified_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    completed_at TIMESTAMP WITH TIME ZONE,
    estimated_duration_minutes INTEGER,
    actual_duration_minutes INTEGER,
    recurrence_pattern VARCHAR(100),
    assigned_to UUID,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    deleted_at TIMESTAMP WITH TIME ZONE,
    
    CONSTRAINT valid_priority_order CHECK (priority_order > 0),
    CONSTRAINT valid_durations CHECK (
        (estimated_duration_minutes IS NULL OR estimated_duration_minutes > 0) AND
        (actual_duration_minutes IS NULL OR actual_duration_minutes > 0)
    )
);

-- Indexes
CREATE INDEX idx_tasks_user_id ON tasks(user_id) WHERE is_deleted = FALSE;
CREATE INDEX idx_tasks_status ON tasks(status) WHERE is_deleted = FALSE;
CREATE INDEX idx_tasks_priority ON tasks(priority, priority_order) WHERE is_deleted = FALSE;
CREATE INDEX idx_tasks_due_date ON tasks(due_date) WHERE is_deleted = FALSE;
CREATE INDEX idx_tasks_created_at ON tasks(created_at DESC);
CREATE INDEX idx_tasks_parent_task_id ON tasks(parent_task_id) WHERE parent_task_id IS NOT NULL;

-- Full-text search
CREATE INDEX idx_tasks_fulltext ON tasks USING GIN (
    to_tsvector('english', COALESCE(title, '') || ' ' || COALESCE(description, ''))
);

-- Tags (many-to-many)
CREATE TABLE task_tags (
    task_id UUID NOT NULL REFERENCES tasks(id) ON DELETE CASCADE,
    tag VARCHAR(50) NOT NULL,
    PRIMARY KEY (task_id, tag)
);

CREATE INDEX idx_task_tags_tag ON task_tags(tag);

-- Categories (many-to-many)
CREATE TABLE task_categories (
    task_id UUID NOT NULL REFERENCES tasks(id) ON DELETE CASCADE,
    category VARCHAR(50) NOT NULL,
    PRIMARY KEY (task_id, category)
);

CREATE INDEX idx_task_categories_category ON task_categories(category);

-- Attachments
CREATE TABLE task_attachments (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    task_id UUID NOT NULL REFERENCES tasks(id) ON DELETE CASCADE,
    file_name VARCHAR(255) NOT NULL,
    file_path VARCHAR(500) NOT NULL,
    file_size_bytes BIGINT NOT NULL,
    content_type VARCHAR(100) NOT NULL,
    thumbnail_path VARCHAR(500),
    extracted_text TEXT,
    uploaded_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    uploaded_by UUID NOT NULL REFERENCES users(id)
);

CREATE INDEX idx_task_attachments_task_id ON task_attachments(task_id);

-- Reminders
CREATE TABLE task_reminders (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    task_id UUID NOT NULL REFERENCES tasks(id) ON DELETE CASCADE,
    reminder_type VARCHAR(50) NOT NULL,
    reminder_time TIMESTAMP WITH TIME ZONE NOT NULL,
    is_sent BOOLEAN NOT NULL DEFAULT FALSE,
    sent_at TIMESTAMP WITH TIME ZONE,
    delivery_channels TEXT[] NOT NULL,
    snooze_until TIMESTAMP WITH TIME ZONE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_task_reminders_task_id ON task_reminders(task_id);
CREATE INDEX idx_task_reminders_time ON task_reminders(reminder_time) WHERE is_sent = FALSE;

-- Dependencies
CREATE TABLE task_dependencies (
    task_id UUID NOT NULL REFERENCES tasks(id) ON DELETE CASCADE,
    depends_on_task_id UUID NOT NULL REFERENCES tasks(id) ON DELETE CASCADE,
    dependency_type VARCHAR(50) NOT NULL DEFAULT 'FinishToStart',
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    
    PRIMARY KEY (task_id, depends_on_task_id),
    CONSTRAINT no_self_dependency CHECK (task_id != depends_on_task_id)
);

CREATE INDEX idx_task_dependencies_depends_on ON task_dependencies(depends_on_task_id);

-- Templates
CREATE TABLE task_templates (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    name VARCHAR(100) NOT NULL,
    description TEXT,
    template_data JSONB NOT NULL,
    is_shared BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    modified_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_task_templates_user_id ON task_templates(user_id);

-- Trigger to update modified_at
CREATE OR REPLACE FUNCTION update_modified_at()
RETURNS TRIGGER AS $$
BEGIN
    NEW.modified_at = NOW();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER tasks_modified_at
    BEFORE UPDATE ON tasks
    FOR EACH ROW
    EXECUTE FUNCTION update_modified_at();

CREATE TRIGGER task_templates_modified_at
    BEFORE UPDATE ON task_templates
    FOR EACH ROW
    EXECUTE FUNCTION update_modified_at();
```

#### Testing Requirements

##### Unit Tests (Target: 85% coverage)
- Task creation validation
- Priority ordering logic
- Recurrence pattern parsing
- Dependency cycle detection
- Search query building
- Business rule validation (e.g., can't complete task with incomplete dependencies)

##### Integration Tests
- Full CRUD operations with database
- File upload and storage
- Reminder scheduling and delivery
- Search functionality with real data
- Concurrent task updates (optimistic locking)

##### Performance Tests
- Load test: 100 concurrent users creating/updating tasks
- Stress test: 10,000 tasks per user
- Search performance with large datasets (100k+ tasks)

---

### 2. Notes & Ideas Service

**Service Name**: `notes-service`  
**Primary Database**: MongoDB  
**Cache**: Redis

#### Functional Requirements

##### FR-NI-001: Note CRUD Operations
- Create, Read, Update, Delete notes
- Each note must have:
  - Unique ID (GUID)
  - Title (optional, max 200 chars)
  - Content (rich text/markdown, max 100,000 chars)
  - Note Type (Idea, Meeting Notes, Quick Note, Journal, Research, Reference)
  - Created Date (auto)
  - Modified Date (auto)
  - Tags (array)
  - Color/Theme (for visual organization)
  - Is Favorite (boolean)
  - Is Archived (boolean)
  - Location (optional geolocation for context)

##### FR-NI-002: Rich Content Support
- Markdown formatting
- Code blocks with syntax highlighting
- Tables
- Checklists (interactive to-do items within notes)
- Mathematical equations (LaTeX)
- Mermaid diagrams (flowcharts, sequence diagrams, etc.)
- Embedded links with preview cards

##### FR-NI-003: Attachments & Media
- Attach multiple files per note
- Inline images with drag-and-drop
- Voice recordings (audio notes)
- Sketches/drawings (canvas-based)
- PDF documents
- Support same file types as task manager
- Generate thumbnails and previews

##### FR-NI-004: Idea Capture Features
- Quick capture mode (minimal fields, rapid entry)
- Voice-to-text transcription
- OCR from images (extract text from photos)
- Web clipper (save articles, links with metadata)
- Email-to-note (send emails to create notes)

##### FR-NI-005: Organization
- Hierarchical notebooks/folders
- Tag-based organization (multiple tags per note)
- Saved searches (smart folders)
- Recently viewed/edited
- Favorites/starred notes

##### FR-NI-006: Search & Discovery
- Full-text search across all notes and attachments
- Filter by date, type, tags, notebook
- Fuzzy search (typo tolerance)
- Search within note content
- Related notes suggestions (based on content similarity)

##### FR-NI-007: Collaboration (Future)
- Share notes with other users (read-only or edit)
- Comments on notes
- Version history (track changes)
- Conflict resolution for concurrent edits

##### FR-NI-008: Export & Integration
- Export individual notes (MD, PDF, DOCX, HTML)
- Bulk export (ZIP archive)
- Integration with task manager (create tasks from notes)
- Webhook notifications for new notes

#### Non-Functional Requirements

##### NFR-NI-001: Performance
- Note save: < 100ms for text, < 2s for large attachments
- Search results: < 200ms
- Support 10,000 notes per user
- Lazy loading for large note lists (virtualization)

##### NFR-NI-002: Offline Support (Mobile)
- Sync notes when online
- Queue changes when offline
- Conflict resolution on sync
- Download notes for offline access

##### NFR-NI-003: Data Retention
- Soft delete (trash bin for 30 days)
- Version history for 90 days
- Permanent delete after trash retention

#### API Endpoints

```
POST   /api/v1/notes                    - Create note
GET    /api/v1/notes                    - List notes
GET    /api/v1/notes/{id}               - Get note
PUT    /api/v1/notes/{id}               - Update note
DELETE /api/v1/notes/{id}               - Delete (soft)
POST   /api/v1/notes/{id}/attachments   - Add attachment
DELETE /api/v1/notes/{id}/attachments/{attachmentId}
GET    /api/v1/notes/search             - Search notes
POST   /api/v1/notes/{id}/favorite      - Toggle favorite
POST   /api/v1/notes/{id}/archive       - Toggle archive
GET    /api/v1/notes/recent             - Recently accessed
POST   /api/v1/notes/quick-capture      - Quick note creation
POST   /api/v1/notes/voice-to-text      - Transcribe audio
POST   /api/v1/notes/ocr                - Extract text from image
GET    /api/v1/notes/{id}/export        - Export note
POST   /api/v1/notes/{id}/create-task   - Create task from note
GET    /api/v1/notes/{id}/versions      - Get version history
POST   /api/v1/notebooks                - Create notebook
GET    /api/v1/notebooks                - List notebooks
```

#### MCP Server Integration

**MCP Server Name**: `notes-mcp`

##### MCP Tools Exposed

```json
{
  "tools": [
    {
      "name": "create_note",
      "description": "Create a new note or idea",
      "inputSchema": {
        "type": "object",
        "properties": {
          "title": { "type": "string" },
          "content": { "type": "string" },
          "noteType": { 
            "type": "string", 
            "enum": ["Idea", "MeetingNotes", "QuickNote", "Journal", "Research", "Reference"] 
          },
          "tags": { "type": "array", "items": { "type": "string" } }
        },
        "required": ["content"]
      }
    },
    {
      "name": "search_notes",
      "description": "Search through all notes",
      "inputSchema": {
        "type": "object",
        "properties": {
          "query": { "type": "string" },
          "filters": { "type": "object" }
        },
        "required": ["query"]
      }
    },
    {
      "name": "get_recent_notes",
      "description": "Get recently accessed notes",
      "inputSchema": {
        "type": "object",
        "properties": {
          "limit": { "type": "integer", "default": 10 }
        }
      }
    },
    {
      "name": "extract_text_from_image",
      "description": "OCR text extraction from image",
      "inputSchema": {
        "type": "object",
        "properties": {
          "imageUrl": { "type": "string" }
        },
        "required": ["imageUrl"]
      }
    },
    {
      "name": "create_task_from_note",
      "description": "Convert note content into a task",
      "inputSchema": {
        "type": "object",
        "properties": {
          "noteId": { "type": "string" },
          "taskProperties": { "type": "object" }
        },
        "required": ["noteId"]
      }
    }
  ]
}
```

#### Database Schema (MongoDB)

```javascript
// Notes collection
{
  _id: UUID,
  userId: UUID,
  notebookId: UUID, // optional
  title: String,
  content: String, // markdown/rich text
  contentType: String, // "markdown", "richtext"
  noteType: String, // "Idea", "MeetingNotes", etc.
  tags: [String],
  color: String,
  isFavorite: Boolean,
  isArchived: Boolean,
  isPinned: Boolean,
  location: {
    latitude: Number,
    longitude: Number,
    address: String
  },
  attachments: [{
    id: UUID,
    fileName: String,
    filePath: String,
    fileSize: Number,
    contentType: String,
    thumbnailPath: String,
    extractedText: String,
    uploadedAt: ISODate
  }],
  checklists: [{
    id: UUID,
    text: String,
    isChecked: Boolean,
    order: Number
  }],
  metadata: {
    wordCount: Number,
    characterCount: Number,
    readingTimeMinutes: Number
  },
  createdAt: ISODate,
  modifiedAt: ISODate,
  lastAccessedAt: ISODate,
  isDeleted: Boolean,
  deletedAt: ISODate,
  
  // Indexes
  indexes: [
    { userId: 1, isDeleted: 1, modifiedAt: -1 },
    { userId: 1, tags: 1 },
    { userId: 1, notebookId: 1 },
    { "$**": "text" } // full-text search
  ]
}

// Notebooks collection
{
  _id: UUID,
  userId: UUID,
  name: String,
  description: String,
  color: String,
  icon: String,
  parentNotebookId: UUID, // for nested notebooks
  order: Number,
  createdAt: ISODate,
  modifiedAt: ISODate,
  isDeleted: Boolean
}

// Note versions (for history)
{
  _id: UUID,
  noteId: UUID,
  userId: UUID,
  content: String,
  versionNumber: Number,
  createdAt: ISODate,
  createdBy: UUID,
  changeDescription: String
}
```

---

### 3. AI Agent Task Queue Service

**Service Name**: `agent-queue-service`  
**Primary Database**: PostgreSQL + Redis  
**Message Queue**: RabbitMQ

#### Functional Requirements

##### FR-AQ-001: Task Queue Management
- Queue tasks for AI agent execution
- Each queued task has:
  - Unique ID (GUID)
  - Task Type (Research, ContentGeneration, DataAnalysis, Summarization, Custom)
  - Task Parameters (JSON)
  - Priority (High, Normal, Low)
  - Status (Queued, InProgress, Completed, Failed, Cancelled)
  - Submitted By (user ID)
  - Assigned Agent (agent ID)
  - Created Date
  - Started Date
  - Completed Date
  - Retry Count
  - Max Retries
  - Timeout (minutes)

##### FR-AQ-002: Agent Task Types

**Research Tasks**
- Web search and summarization
- Competitor analysis
- Market research
- Academic paper reviews
- Output: Structured document (MD, DOCX, PDF)

**Content Generation Tasks**
- Blog posts, articles
- Email drafts
- Social media content
- Report generation
- Presentation creation
- Output: Formatted documents

**Data Analysis Tasks**
- Excel data analysis
- Report generation from data
- Trend analysis
- Visualization creation
- Output: Reports with charts/graphs

**Summarization Tasks**
- Document summarization
- Meeting transcription summaries
- Email thread summaries
- Output: Concise summaries

**Custom Tasks**
- User-defined workflows
- API integrations
- Custom scripts/automations

##### FR-AQ-003: Task Submission
- Submit from mobile app
- Submit from web UI
- Submit via API
- Submit via MCP tools (from AI assistants)
- Batch submission
- Scheduled tasks (run at specific time)

##### FR-AQ-004: Progress Tracking
- Real-time progress updates
- Progress percentage (0-100%)
- Status messages (what agent is doing)
- Estimated completion time
- WebSocket/SignalR for live updates

##### FR-AQ-005: Result Handling
- Store results in appropriate service (documents in notes, files in storage)
- Notification on completion (push, email, webhook)
- Result preview in app
- Download results
- Auto-integration (e.g., research results auto-create note)

##### FR-AQ-006: Error Handling
- Automatic retry with exponential backoff
- Partial result saving (if task fails midway)
- Error logging with details
- User notification on failure
- Manual retry option

##### FR-AQ-007: Queue Management
- View queue status
- Prioritize queued tasks
- Cancel queued/in-progress tasks
- Pause/resume queue processing
- Queue statistics (average time, success rate)

#### Non-Functional Requirements

##### NFR-AQ-001: Scalability
- Support 1000+ concurrent queue items
- Horizontal scaling of worker processes
- Load balancing across agent workers
- Queue depth monitoring

##### NFR-AQ-002: Reliability
- Persistent queue (survives restarts)
- Dead letter queue for repeated failures
- Transaction support (atomic operations)
- Idempotency (retry-safe)

##### NFR-AQ-003: Performance
- Task pickup latency: < 1 second
- Status update latency: < 500ms
- Support long-running tasks (up to 24 hours)

#### API Endpoints

```
POST   /api/v1/agent-tasks              - Submit task
GET    /api/v1/agent-tasks              - List queued tasks
GET    /api/v1/agent-tasks/{id}         - Get task status
DELETE /api/v1/agent-tasks/{id}         - Cancel task
POST   /api/v1/agent-tasks/{id}/retry   - Retry failed task
GET    /api/v1/agent-tasks/{id}/result  - Get task result
GET    /api/v1/agent-tasks/{id}/progress - Get progress (SSE)
POST   /api/v1/agent-tasks/batch        - Batch submit
GET    /api/v1/agent-tasks/stats        - Queue statistics
POST   /api/v1/agent-tasks/schedule     - Schedule task
GET    /api/v1/agent-tasks/scheduled    - List scheduled tasks
```

#### MCP Server Integration

**MCP Server Name**: `agent-queue-mcp`

##### MCP Tools Exposed

```json
{
  "tools": [
    {
      "name": "queue_research_task",
      "description": "Queue a research task for AI agent",
      "inputSchema": {
        "type": "object",
        "properties": {
          "topic": { "type": "string" },
          "depth": { "type": "string", "enum": ["quick", "standard", "comprehensive"] },
          "outputFormat": { "type": "string", "enum": ["markdown", "pdf", "docx"] },
          "sources": { "type": "array", "items": { "type": "string" } }
        },
        "required": ["topic"]
      }
    },
    {
      "name": "queue_content_generation",
      "description": "Queue content generation task",
      "inputSchema": {
        "type": "object",
        "properties": {
          "contentType": { "type": "string", "enum": ["blog", "email", "social", "report"] },
          "prompt": { "type": "string" },
          "length": { "type": "string", "enum": ["short", "medium", "long"] },
          "tone": { "type": "string" }
        },
        "required": ["contentType", "prompt"]
      }
    },
    {
      "name": "get_task_status",
      "description": "Check status of queued task",
      "inputSchema": {
        "type": "object",
        "properties": {
          "taskId": { "type": "string" }
        },
        "required": ["taskId"]
      }
    },
    {
      "name": "get_task_result",
      "description": "Retrieve completed task result",
      "inputSchema": {
        "type": "object",
        "properties": {
          "taskId": { "type": "string" }
        },
        "required": ["taskId"]
      }
    }
  ]
}
```

#### Database Schema

```sql
-- Agent tasks queue
CREATE TABLE agent_tasks (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users(id),
    task_type VARCHAR(50) NOT NULL,
    task_parameters JSONB NOT NULL,
    priority VARCHAR(20) NOT NULL DEFAULT 'Normal',
    status VARCHAR(50) NOT NULL DEFAULT 'Queued',
    assigned_agent_id UUID,
    result_data JSONB,
    result_files TEXT[], -- array of file paths
    error_message TEXT,
    error_stack_trace TEXT,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    started_at TIMESTAMP WITH TIME ZONE,
    completed_at TIMESTAMP WITH TIME ZONE,
    retry_count INTEGER NOT NULL DEFAULT 0,
    max_retries INTEGER NOT NULL DEFAULT 3,
    timeout_minutes INTEGER NOT NULL DEFAULT 60,
    progress_percentage INTEGER DEFAULT 0,
    progress_message TEXT,
    scheduled_for TIMESTAMP WITH TIME ZONE,
    
    CONSTRAINT valid_progress CHECK (progress_percentage >= 0 AND progress_percentage <= 100),
    CONSTRAINT valid_retry CHECK (retry_count >= 0 AND retry_count <= max_retries)
);

CREATE INDEX idx_agent_tasks_user_id ON agent_tasks(user_id);
CREATE INDEX idx_agent_tasks_status ON agent_tasks(status);
CREATE INDEX idx_agent_tasks_priority ON agent_tasks(priority, created_at);
CREATE INDEX idx_agent_tasks_scheduled ON agent_tasks(scheduled_for) WHERE scheduled_for IS NOT NULL;

-- Task execution logs
CREATE TABLE agent_task_logs (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    task_id UUID NOT NULL REFERENCES agent_tasks(id) ON DELETE CASCADE,
    log_level VARCHAR(20) NOT NULL,
    message TEXT NOT NULL,
    metadata JSONB,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_agent_task_logs_task_id ON agent_task_logs(task_id, created_at DESC);
```

---

### 4. Mobile Application (.NET MAUI)

**App Name**: `BusinessAgentMobile`  
**Platform**: Android (primary), iOS (secondary)  
**Sideload Support**: Yes (APK distribution)

#### Functional Requirements

##### FR-MA-001: Core Features
- View and manage tasks
- Create/view notes
- Queue AI agent tasks
- View queue status and results
- Push notifications
- Offline mode (basic functionality)

##### FR-MA-002: Task Management UI
- Task list with filtering
- Swipe actions (complete, delete, reschedule)
- Quick add task (voice input supported)
- Task details view
- Drag-to-reorder within priorities
- Calendar view for scheduled tasks

##### FR-MA-003: Notes UI
- Note list (grid or list view)
- Rich text editor
- Voice-to-text input
- Camera integration (take photo → OCR → note)
- Quick capture widget
- Search notes

##### FR-MA-004: AI Agent Queue UI
- Submit research request
- Submit content generation request
- View queue status
- Real-time progress for in-progress tasks
- View/download completed results
- Templates for common requests

##### FR-MA-005: Authentication
- JWT token-based auth
- Biometric login (fingerprint, face ID)
- Remember me (secure token storage)
- Session timeout (configurable)

##### FR-MA-006: Offline Support
- Cache recent tasks/notes
- Queue actions for sync
- Conflict resolution on reconnect
- Indicate offline status clearly

##### FR-MA-007: Notifications
- Firebase Cloud Messaging (FCM)
- Task reminders
- Agent task completion
- Custom notification actions (complete task, view result)

##### FR-MA-008: Settings
- Server endpoint configuration
- Notification preferences
- Sync frequency
- Cache size limits
- Theme (light/dark/auto)

#### Non-Functional Requirements

##### NFR-MA-001: Performance
- App launch: < 2 seconds
- Smooth scrolling (60 FPS)
- Image loading: progressive with placeholders
- Efficient battery usage

##### NFR-MA-002: UX
- Material Design 3 (Android)
- Responsive layouts (phones and tablets)
- Accessibility support (screen readers, large text)
- Haptic feedback for interactions

##### NFR-MA-003: Security
- Secure storage for auth tokens
- Certificate pinning for API calls
- No sensitive data in logs
- Encrypted local database (if offline storage)

#### Key Libraries/Packages

- **CommunityToolkit.Maui** - Extended UI controls
- **CommunityToolkit.Mvvm** - MVVM framework
- **Refit** - REST API client
- **Microsoft.AspNetCore.SignalR.Client** - Real-time updates
- **Akavache** - Async cache
- **FFImageLoading.Maui** - Image loading/caching
- **Plugin.LocalNotification** - Local notifications
- **Plugin.Fingerprint** - Biometric auth
- **SkiaSharp** - Custom UI rendering

---

### 5. Supporting Services

#### 5.1 Authentication & User Service

**Service Name**: `auth-service`  
**Database**: PostgreSQL

##### Features
- User registration/login
- JWT token generation/validation
- Refresh token support
- Password reset (email-based)
- User profile management
- Multi-factor authentication (TOTP)
- API key management (for programmatic access)
- Audit logging (login history)

##### Database Schema

```sql
CREATE TABLE users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    email VARCHAR(255) NOT NULL UNIQUE,
    username VARCHAR(100) NOT NULL UNIQUE,
    password_hash VARCHAR(500) NOT NULL,
    full_name VARCHAR(200),
    avatar_url VARCHAR(500),
    phone_number VARCHAR(20),
    is_email_verified BOOLEAN NOT NULL DEFAULT FALSE,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    modified_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    last_login_at TIMESTAMP WITH TIME ZONE,
    failed_login_attempts INTEGER NOT NULL DEFAULT 0,
    locked_until TIMESTAMP WITH TIME ZONE,
    
    CONSTRAINT valid_email CHECK (email ~* '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}$')
);

CREATE TABLE refresh_tokens (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    token VARCHAR(500) NOT NULL UNIQUE,
    expires_at TIMESTAMP WITH TIME ZONE NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    revoked_at TIMESTAMP WITH TIME ZONE,
    replaced_by_token VARCHAR(500)
);

CREATE INDEX idx_refresh_tokens_user_id ON refresh_tokens(user_id);
CREATE INDEX idx_refresh_tokens_token ON refresh_tokens(token) WHERE revoked_at IS NULL;

CREATE TABLE user_roles (
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    role VARCHAR(50) NOT NULL,
    PRIMARY KEY (user_id, role)
);

CREATE TABLE api_keys (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    key_name VARCHAR(100) NOT NULL,
    key_hash VARCHAR(500) NOT NULL,
    prefix VARCHAR(20) NOT NULL,
    scopes TEXT[] NOT NULL,
    expires_at TIMESTAMP WITH TIME ZONE,
    last_used_at TIMESTAMP WITH TIME ZONE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    is_revoked BOOLEAN NOT NULL DEFAULT FALSE,
    revoked_at TIMESTAMP WITH TIME ZONE
);
```

#### 5.2 Notification Service

**Service Name**: `notification-service`  
**Database**: PostgreSQL + Redis  
**Message Queue**: RabbitMQ

##### Features
- Multi-channel delivery (email, push, SMS, webhook)
- Template management
- Scheduling
- Retry logic
- Delivery tracking
- User preferences (opt-in/opt-out per channel)

##### Integrations
- **Email**: SendGrid or Amazon SES
- **Push**: Firebase Cloud Messaging
- **SMS**: Twilio (future)

#### 5.3 File Storage Service

**Service Name**: `storage-service`  
**Storage**: MinIO (S3-compatible) or Azure Blob

##### Features
- Upload files (chunked for large files)
- Download files (with resume support)
- Generate signed URLs (temporary access)
- Image resizing/thumbnails
- Virus scanning (ClamAV)
- Quota management per user
- File retention policies

#### 5.4 API Gateway

**Technology**: Ocelot or YARP (Yet Another Reverse Proxy)

##### Features
- Route requests to appropriate services
- Rate limiting (per user, per endpoint)
- Request/response logging
- API versioning
- Load balancing
- Circuit breaker
- Request aggregation (combine multiple backend calls)
- JWT validation

---

## Infrastructure Architecture

### Development Environment

**Docker Compose Setup** (`docker-compose.yml`)

Services:
- `postgres` - PostgreSQL 16
- `mongodb` - MongoDB 7
- `redis` - Redis 7
- `rabbitmq` - RabbitMQ with management plugin
- `minio` - Object storage
- `seq` - Log aggregation
- `jaeger` - Distributed tracing
- All application services (each as separate container)

**Networking**:
- Private network for inter-service communication
- Exposed ports only for: API Gateway (80, 443), Seq UI (5341), Jaeger UI (16686)

### Production Environment

#### Kubernetes Deployment

**Cluster Requirements**:
- Minimum 3 nodes
- Node size: 8 vCPU, 16GB RAM per node
- Storage: Dynamic provisioning (for persistent volumes)

**Namespaces**:
- `production` - Production workloads
- `staging` - Staging environment
- `monitoring` - Monitoring stack

**Helm Charts** (one per service):
- Deployments (with HPA - Horizontal Pod Autoscaler)
- Services (ClusterIP or LoadBalancer)
- ConfigMaps and Secrets
- Ingress rules (NGINX Ingress Controller)

**Ingress Configuration**:
```yaml
apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: api-gateway-ingress
  annotations:
    cert-manager.io/cluster-issuer: letsencrypt-prod
    nginx.ingress.kubernetes.io/rate-limit: "100"
spec:
  ingressClassName: nginx
  tls:
  - hosts:
    - api.yourdomain.com
    secretName: api-tls
  rules:
  - host: api.yourdomain.com
    http:
      paths:
      - path: /
        pathType: Prefix
        backend:
          service:
            name: api-gateway
            port:
              number: 80
```

#### Terraform Infrastructure

**Cloud Provider**: AWS, Azure, or GCP (specify in modules)

**Resources to Provision**:
- Kubernetes cluster (EKS, AKS, or GKE)
- Managed databases (RDS PostgreSQL, DocumentDB/Cosmos DB for MongoDB)
- Managed Redis (ElastiCache, Azure Cache)
- Object storage (S3, Azure Blob)
- Load balancers
- VPC/Virtual Network with subnets
- Security groups/Network policies
- IAM roles and policies
- Monitoring (CloudWatch, Azure Monitor, or Stackdriver)

**Terraform Structure**:
```
terraform/
├── modules/
│   ├── networking/
│   ├── kubernetes/
│   ├── databases/
│   ├── storage/
│   └── monitoring/
├── environments/
│   ├── dev/
│   ├── staging/
│   └── production/
└── main.tf
```

### CI/CD Pipeline

**GitHub Actions Workflow** (`.github/workflows/`)

**On Pull Request**:
1. Run unit tests
2. Run integration tests (with Testcontainers)
3. Code quality checks (SonarQube)
4. Security scanning (Snyk, Trivy)
5. Build Docker images

**On Merge to Main**:
1. Run all tests
2. Build and tag Docker images
3. Push to container registry
4. Deploy to staging
5. Run E2E tests in staging
6. Manual approval for production
7. Deploy to production (blue-green or canary)
8. Run smoke tests

**Tools**:
- **Container Registry**: Docker Hub, GitHub Container Registry, or ECR/ACR
- **Secrets Management**: GitHub Secrets → Kubernetes Secrets (via sealed-secrets or external-secrets)

---

## Observability & Monitoring

### Logging

**Stack**: Seq (dev) + ELK/Loki (production)

**Log Levels**:
- **Error**: Exceptions, failures
- **Warning**: Degraded performance, retry attempts
- **Information**: Important business events (task created, note saved)
- **Debug**: Detailed diagnostic info (dev only)

**Structured Logging**: Use Serilog with consistent properties
- `UserId`
- `CorrelationId` (trace requests across services)
- `ServiceName`
- `Environment`

### Metrics

**Stack**: Prometheus + Grafana

**Key Metrics**:
- Request rate (per endpoint)
- Response times (p50, p95, p99)
- Error rate (4xx, 5xx)
- Database connection pool stats
- Queue depth and processing rate
- Cache hit/miss ratio
- Disk/memory usage

**Custom Metrics**:
- Tasks created per hour
- Notes created per hour
- Agent tasks queued/completed
- User active sessions

### Distributed Tracing

**Stack**: OpenTelemetry + Jaeger

**Trace Context Propagation**:
- Generate trace ID at API gateway
- Pass through all services
- Correlate with logs

### Alerting

**Stack**: Prometheus Alertmanager

**Critical Alerts**:
- Service down (> 5 minutes)
- Error rate > 5%
- Response time p99 > 2 seconds
- Database connection failures
- Disk usage > 85%
- Memory usage > 90%

**Notification Channels**:
- PagerDuty (critical)
- Slack (warnings)
- Email (info)

---

## Security Requirements

### Application Security (OWASP Top 10)

1. **Injection Prevention**
   - Parameterized queries (EF Core, Dapper)
   - Input validation (FluentValidation)
   - Output encoding

2. **Authentication & Authorization**
   - JWT with short expiry (15 minutes)
   - Refresh tokens (secure, HttpOnly cookies)
   - Password hashing (BCrypt or Argon2)
   - MFA support

3. **Sensitive Data Exposure**
   - TLS 1.3 for all communications
   - Encrypt database backups
   - No secrets in code/config (use Key Vault)
   - Mask sensitive data in logs

4. **XXE & SSRF Prevention**
   - Disable external entity resolution in XML parsers
   - Validate and sanitize URLs in web fetch

5. **Broken Access Control**
   - Verify user owns resources (multi-tenant isolation)
   - Implement RBAC
   - Deny by default

6. **Security Misconfiguration**
   - Disable unnecessary features/ports
   - Remove default credentials
   - Keep dependencies updated (Dependabot)

7. **XSS Prevention**
   - Content Security Policy headers
   - Sanitize user inputs
   - Use framework protections (Razor, React)

8. **Insecure Deserialization**
   - Validate JSON schemas
   - Use safe serializers
   - Limit accepted content types

9. **Logging & Monitoring**
   - Log security events (failed logins, access denied)
   - Monitor for anomalies
   - Audit trail for sensitive operations

10. **Dependency Management**
    - Regular updates
    - Vulnerability scanning (npm audit, dotnet list package --vulnerable)

### API Security

- **Rate Limiting**: Per user and per endpoint
- **CORS**: Whitelist allowed origins
- **API Keys**: For programmatic access (separate from user auth)
- **Request Size Limits**: Prevent DoS
- **Input Validation**: Strict schemas for all endpoints

### Compliance

- **WCAG 2.1 Level AA**: For web UI and mobile app
- **GDPR** (if applicable):
  - Right to access data
  - Right to deletion
  - Data portability
  - Consent management

---

## Testing Strategy

### Unit Tests

**Framework**: xUnit  
**Tools**: FluentAssertions, Moq, AutoFixture

**Coverage Target**: 80% minimum

**Focus Areas**:
- Business logic
- Validation rules
- Utility functions
- Domain models

**Example**:
```csharp
[Fact]
public void CreateTask_WithValidData_ReturnsTask()
{
    // Arrange
    var service = new TaskService(mockRepo.Object);
    var request = new CreateTaskRequest { Title = "Test", Priority = "A" };
    
    // Act
    var result = service.CreateTask(request);
    
    // Assert
    result.Should().NotBeNull();
    result.Title.Should().Be("Test");
}
```

### Integration Tests

**Framework**: xUnit + WebApplicationFactory  
**Tools**: Testcontainers (for databases), WireMock (for external APIs)

**Coverage**:
- API endpoints (full CRUD)
- Database interactions
- Message queue publishing/consumption
- File uploads

**Example**:
```csharp
public class TaskApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task CreateTask_ReturnsCreatedTask()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new CreateTaskRequest { Title = "Integration Test" };
        
        // Act
        var response = await client.PostAsJsonAsync("/api/v1/tasks", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var task = await response.Content.ReadFromJsonAsync<TaskDto>();
        task.Title.Should().Be("Integration Test");
    }
}
```

### E2E Tests

**Framework**: Playwright (web), Appium (mobile)

**Scenarios**:
- User registration and login
- Create task flow
- Create note flow
- Queue AI agent task flow
- Search functionality
- Mobile app critical paths

### Performance Tests

**Tools**: k6, JMeter, or NBomber

**Load Tests**:
- 100 concurrent users
- Sustained for 30 minutes
- Verify response times remain acceptable

**Stress Tests**:
- Gradually increase load to breaking point
- Identify bottlenecks

**Soak Tests**:
- Run at 80% capacity for 24 hours
- Check for memory leaks

---

## Documentation Requirements

### Code Documentation

- XML documentation comments for public APIs
- README.md in each service repository
- Architecture Decision Records (ADRs) for major decisions

### API Documentation

- OpenAPI/Swagger specifications
- Hosted Swagger UI (dev environment)
- Example requests/responses
- Error codes and meanings

### User Documentation

- User guide (web and mobile)
- Video tutorials (optional)
- FAQ
- Troubleshooting guide

### Developer Documentation

- Setup instructions (development environment)
- Contributing guidelines
- Coding standards
- Deployment guide
- Runbooks for common issues

---

## Project Structure

### Repository Layout

**Monorepo or Multi-repo?**: Recommend monorepo for cohesion

```
ai-business-agent/
├── src/
│   ├── services/
│   │   ├── TaskManager/
│   │   │   ├── TaskManager.Api/
│   │   │   ├── TaskManager.Core/
│   │   │   ├── TaskManager.Infrastructure/
│   │   │   └── TaskManager.Tests/
│   │   ├── Notes/
│   │   ├── AgentQueue/
│   │   ├── Auth/
│   │   ├── Notifications/
│   │   └── Storage/
│   ├── mobile/
│   │   └── BusinessAgentMobile/
│   ├── shared/
│   │   ├── Common/
│   │   ├── Contracts/
│   │   └── EventBus/
│   └── gateway/
│       └── ApiGateway/
├── infrastructure/
│   ├── docker/
│   │   └── docker-compose.yml
│   ├── kubernetes/
│   │   └── helm-charts/
│   └── terraform/
├── tests/
│   ├── integration/
│   ├── e2e/
│   └── performance/
├── docs/
│   ├── architecture/
│   ├── api/
│   └── user-guides/
├── .github/
│   └── workflows/
└── README.md
```

### Service Structure (Clean Architecture)

```
TaskManager.Api/
├── Controllers/
├── Middleware/
├── Program.cs
└── appsettings.json

TaskManager.Core/
├── Domain/
│   ├── Entities/
│   ├── ValueObjects/
│   └── Exceptions/
├── Application/
│   ├── Commands/
│   ├── Queries/
│   ├── DTOs/
│   ├── Interfaces/
│   └── Validators/
└── Shared/

TaskManager.Infrastructure/
├── Persistence/
│   ├── Repositories/
│   ├── Configurations/
│   └── Migrations/
├── ExternalServices/
└── DependencyInjection.cs

TaskManager.Tests/
├── Unit/
├── Integration/
└── Fixtures/
```

---

## Development Phases

### Phase 1: Foundation (Weeks 1-3)
- [ ] Project setup (repos, CI/CD)
- [ ] Infrastructure provisioning (Terraform)
- [ ] Auth service
- [ ] API Gateway
- [ ] Docker Compose environment
- [ ] Base Helm charts

### Phase 2: Core Services (Weeks 4-8)
- [ ] Task Management Service (full CRUD)
- [ ] Notes Service (full CRUD)
- [ ] File Storage Service
- [ ] Unit tests for core services

### Phase 3: AI Integration (Weeks 9-12)
- [ ] Agent Queue Service
- [ ] MCP servers for all services
- [ ] Background workers for agent tasks
- [ ] Integration tests

### Phase 4: Mobile Application (Weeks 13-16)
- [ ] MAUI project setup
- [ ] Authentication screens
- [ ] Task management UI
- [ ] Notes UI
- [ ] Agent queue UI
- [ ] Offline support

### Phase 5: Advanced Features (Weeks 17-20)
- [ ] Smart scheduling
- [ ] Voice-to-text
- [ ] OCR
- [ ] Advanced search
- [ ] Notifications service

### Phase 6: Polish & Deployment (Weeks 21-24)
- [ ] E2E testing
- [ ] Performance optimization
- [ ] Security audit
- [ ] Documentation
- [ ] Production deployment
- [ ] User acceptance testing

---

## Success Criteria

1. **Functionality**: All core features implemented and tested
2. **Performance**: Meets defined SLAs (response times, throughput)
3. **Quality**: 80%+ test coverage, < 5% error rate
4. **Security**: Pass OWASP top 10 checklist, penetration testing
5. **Usability**: WCAG 2.1 Level AA compliance
6. **Deployment**: Successful production deployment with zero downtime
7. **Documentation**: Complete API docs, user guides, developer guides

---

## Budget & Resource Estimates

### Infrastructure Costs (Monthly, AWS example)

- **EKS Cluster**: $73 (control plane)
- **EC2 Instances**: 3x t3.large = $225
- **RDS PostgreSQL**: db.t3.medium = $85
- **DocumentDB**: 1x r5.large = $200
- **ElastiCache Redis**: cache.t3.medium = $65
- **S3 Storage**: 100GB = $2.50
- **Data Transfer**: ~$50
- **Total**: ~$700/month

### Development Team

- **Backend Developer** (C#/.NET): 600-800 hours
- **Mobile Developer** (MAUI): 300-400 hours
- **DevOps Engineer**: 200-300 hours
- **QA Engineer**: 200-300 hours

**Total Effort**: ~1,500-2,000 hours (~6-9 months with 1-2 developers)

---

## Risks & Mitigations

| Risk | Impact | Probability | Mitigation |
|------|--------|-------------|------------|
| Scope creep | High | Medium | Strict phase boundaries, MVP first |
| Performance issues with large data | Medium | Medium | Early load testing, caching strategy |
| AI agent reliability | High | Medium | Retry logic, fallback mechanisms |
| Security vulnerabilities | High | Low | Regular security audits, automated scanning |
| Kubernetes complexity | Medium | Medium | Start with Docker Compose, migrate gradually |
| Mobile platform differences | Low | Medium | Focus on Android first, abstract platform-specific code |

---

## Next Steps

1. **Review & Refine**: Stakeholder review of requirements
2. **Prioritize**: Rank features by importance (MoSCoW method)
3. **Break Down**: Create detailed user stories and tasks
4. **Assign**: Distribute work to AI assistants or developers
5. **Iterate**: Build in sprints, gather feedback, adjust

---

## Appendix

### Glossary

- **MCP**: Model Context Protocol - interface for AI agents to interact with applications
- **CRUD**: Create, Read, Update, Delete
- **CQRS**: Command Query Responsibility Segregation
- **JWT**: JSON Web Token
- **WCAG**: Web Content Accessibility Guidelines
- **OWASP**: Open Web Application Security Project

### References

- [.NET Documentation](https://docs.microsoft.com/dotnet)
- [ASP.NET Core Best Practices](https://docs.microsoft.com/aspnet/core/fundamentals/best-practices)
- [Kubernetes Documentation](https://kubernetes.io/docs)
- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [Model Context Protocol Specification](https://modelcontextprotocol.io)

---

**Document End**
