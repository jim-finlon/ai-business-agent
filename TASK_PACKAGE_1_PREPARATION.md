# Task Package 1: Database Infrastructure Setup - Preparation Guide

**Date:** October 26, 2025  
**Status:** Ready to Start  
**Estimated Effort:** 8-12 hours

---

## ðŸŽ¯ Objective

Set up all database infrastructure and initial schemas for development environment.

---

## ðŸ“‹ Prerequisites Verification

### âœ… Development Environment
- [x] **Docker 28.5.1** - Installed and working
- [x] **Docker Compose v2.40.0** - Installed and working
- [x] **.NET 9 SDK 9.0.306** - Installed and working
- [x] **Git** - Repository initialized
- [x] **Project Structure** - Created according to specifications

### âœ… Project Setup
- [x] Project directory structure created
- [x] Git repository initialized with initial commit
- [x] .gitignore configured for .NET projects
- [x] README.md and STATUS.md created

---

## ðŸ—ï¸ Task Package 1 Requirements

### 1. Docker Compose Configuration
Create infrastructure/docker/docker-compose.yml with:

**Services Required:**
- **PostgreSQL 16** - Primary relational database
- **MongoDB 7** - Document database for notes
- **Redis 7** - Caching and session management
- **RabbitMQ 3.12** - Message queue with management UI
- **MinIO** - S3-compatible object storage

**Configuration Requirements:**
- Proper networking between containers
- Volume mounts for data persistence
- Environment variables for configuration
- Health checks for all services
- Exposed ports for development access

### 2. PostgreSQL Database Setup
**Databases to Create:**
- 	askmanager_db - Task management service
- uth_db - Authentication service
- gentqueue_db - Agent queue service

**Schema Requirements:**
- Complete schemas from SYSTEM_REQUIREMENTS.md
- All tables, indexes, and constraints
- Triggers for automatic timestamp updates
- Full-text search indexes
- Foreign key relationships

### 3. MongoDB Setup
**Database:** 
otes_db
**Collections:**
- 
otes - Main notes collection
- 
otebooks - Notebook/folder organization
- 
ote_versions - Version history

**Indexes Required:**
- User-based queries
- Full-text search
- Tag-based filtering
- Date-based queries

### 4. Redis Configuration
**Features:**
- Persistence (AOF)
- Memory limits
- Keyspace notifications for cache invalidation
- Session storage configuration

### 5. RabbitMQ Setup
**Features:**
- Management UI enabled
- Virtual hosts for different environments
- Queue definitions
- Exchange configurations

### 6. MinIO Setup
**Buckets to Create:**
- 	asks-attachments - Task file attachments
- 
otes-attachments - Note file attachments
- gent-results - AI agent output files

**Configuration:**
- Access keys for development
- Bucket policies
- CORS configuration

---

## ðŸ“ Files to Create

`
infrastructure/docker/
â”œâ”€â”€ docker-compose.yml
â”œâ”€â”€ docker-compose.override.yml (local dev overrides)
â”œâ”€â”€ postgres/
â”‚   â”œâ”€â”€ init.sql
â”‚   â””â”€â”€ seed-data.sql
â”œâ”€â”€ mongodb/
â”‚   â””â”€â”€ init.js
â”œâ”€â”€ redis/
â”‚   â””â”€â”€ redis.conf
â”œâ”€â”€ rabbitmq/
â”‚   â””â”€â”€ rabbitmq.conf
â””â”€â”€ minio/
    â””â”€â”€ minio.conf
`

---

## âœ… Acceptance Criteria

- [ ] All containers start successfully with docker-compose up
- [ ] Can connect to each database from host machine
- [ ] PostgreSQL schemas created and validated
- [ ] MongoDB indexes created
- [ ] Redis accepts connections
- [ ] RabbitMQ management UI accessible
- [ ] MinIO buckets created and accessible
- [ ] Sample data exists in all databases
- [ ] All services communicate properly
- [ ] Health checks pass for all services

---

## ðŸ§ª Testing Commands

### Docker Compose
`ash
# Start all services
docker-compose up -d

# Check service status
docker-compose ps

# View logs
docker-compose logs

# Stop services
docker-compose down
`

### Database Connectivity
`ash
# PostgreSQL
psql -h localhost -p 5432 -U postgres -d taskmanager_db

# MongoDB
mongosh mongodb://localhost:27017/notes_db

# Redis
redis-cli -h localhost -p 6379

# RabbitMQ Management
# Open http://localhost:15672 in browser

# MinIO Console
# Open http://localhost:9001 in browser
`

---

## ðŸ“š Reference Documentation

- **SYSTEM_REQUIREMENTS.md** - Complete technical specification
- **TASK_BREAKDOWN.md** - Task Package 1 details
- **PostgreSQL 16 Documentation** - https://www.postgresql.org/docs/16/
- **MongoDB 7 Documentation** - https://docs.mongodb.com/v7.0/
- **Redis 7 Documentation** - https://redis.io/docs/
- **RabbitMQ Documentation** - https://www.rabbitmq.com/documentation.html
- **MinIO Documentation** - https://docs.min.io/

---

## ðŸš€ Next Steps After Completion

1. **Task Package 2:** Authentication Service
2. **Task Package 3:** Task Management Service
3. **Task Package 4:** Notes Service

---

**Ready to Start Task Package 1!**

---

**Document End**
