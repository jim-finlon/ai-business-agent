# Database Infrastructure Setup - Task Package 1

**Status:** Complete  
**Date:** October 26, 2025

This directory contains all the configuration files and scripts needed to set up the complete database infrastructure for the AI Business Management System.

---

## 🏗️ Infrastructure Components

### Databases
- **PostgreSQL 16** - Primary relational database
- **MongoDB 7** - Document database for notes
- **Redis 7** - Caching and session management
- **RabbitMQ 3.12** - Message queue with management UI
- **MinIO** - S3-compatible object storage

---

## 🚀 Quick Start

### Prerequisites
- Docker 24+ and Docker Compose
- Git

### Start All Services
```bash
# Navigate to the docker directory
cd infrastructure/docker

# Start all services
docker-compose up -d

# Check service status
docker-compose ps

# View logs
docker-compose logs
```

### Stop Services
```bash
docker-compose down
```

---

## 📊 Service Details

### PostgreSQL (Port 5432)
- **Databases:** `taskmanager_db`, `auth_db`, `agentqueue_db`
- **User:** `postgres` / `postgres123`
- **Features:** Full schemas, indexes, triggers, seed data

### MongoDB (Port 27017)
- **Database:** `notes_db`
- **User:** `root` / `root123`
- **Features:** Collections, indexes, validation, sample data

### Redis (Port 6379)
- **Features:** Persistence, memory management, keyspace notifications
- **Configuration:** Optimized for caching and session management

### RabbitMQ (Ports 5672, 15672)
- **User:** `admin` / `admin123`
- **Management UI:** http://localhost:15672
- **Features:** Message queuing, management interface

### MinIO (Ports 9000, 9001)
- **User:** `minioadmin` / `minioadmin123`
- **Console:** http://localhost:9001
- **API:** http://localhost:9000
- **Buckets:** `tasks-attachments`, `notes-attachments`, `agent-results`

---

## 🔧 Configuration Files

### Docker Compose
- `docker-compose.yml` - Main configuration
- `docker-compose.override.yml` - Development overrides

### Database Configurations
- `postgres/init.sql` - PostgreSQL schemas and tables
- `postgres/seed-data.sql` - Sample data for development
- `mongodb/init.js` - MongoDB collections and indexes
- `redis/redis.conf` - Redis configuration
- `rabbitmq/rabbitmq.conf` - RabbitMQ configuration
- `minio/minio.conf` - MinIO configuration

### Setup Scripts
- `minio/setup-buckets.sh` - MinIO bucket initialization

---

## 🧪 Testing Connectivity

### PostgreSQL
```bash
# Connect to PostgreSQL
psql -h localhost -p 5432 -U postgres -d taskmanager_db

# Test queries
\dt  # List tables
SELECT COUNT(*) FROM tasks;
```

### MongoDB
```bash
# Connect to MongoDB
mongosh mongodb://root:root123@localhost:27017/notes_db

# Test queries
db.notes.countDocuments()
db.notebooks.find()
```

### Redis
```bash
# Connect to Redis
redis-cli -h localhost -p 6379

# Test commands
PING
SET test "Hello Redis"
GET test
```

### RabbitMQ
- **Management UI:** http://localhost:15672
- **Credentials:** admin / admin123

### MinIO
- **Console:** http://localhost:9001
- **Credentials:** minioadmin / minioadmin123

---

## 📈 Sample Data

### Users (auth_db)
- `admin@example.com` / `admin` - System Administrator
- `test@example.com` / `testuser` - Test User
- `demo@example.com` / `demouser` - Demo User

### Tasks (taskmanager_db)
- Sample tasks with different priorities (A, B, C)
- Tasks with dependencies
- Tasks with attachments and reminders
- Task templates

### Notes (notes_db)
- Sample notebooks and notes
- Notes with tags and categories
- Notes with checklists
- Full-text search examples

### Agent Tasks (agentqueue_db)
- Sample research tasks
- Content generation tasks
- Task execution logs

---

## 🔍 Health Checks

All services include health checks that verify:
- Service is running and responsive
- Database connections are working
- Required ports are accessible

Check health status:
```bash
docker-compose ps
```

---

## 📝 Development Notes

### Data Persistence
- All data is persisted in Docker volumes
- Data survives container restarts
- Volumes are named: `postgres_data`, `mongodb_data`, `redis_data`, `rabbitmq_data`, `minio_data`

### Network Configuration
- All services are on the `ai-business-network`
- Services can communicate using container names
- External access via localhost ports

### Security
- Default passwords are used for development
- Change passwords for production deployment
- Enable SSL/TLS for production

---

## 🚨 Troubleshooting

### Common Issues

1. **Port Conflicts**
   ```bash
   # Check if ports are in use
   netstat -tulpn | grep :5432
   
   # Change ports in docker-compose.yml if needed
   ```

2. **Permission Issues**
   ```bash
   # Fix file permissions
   chmod +x minio/setup-buckets.sh
   ```

3. **Service Not Starting**
   ```bash
   # Check logs
   docker-compose logs [service-name]
   
   # Restart specific service
   docker-compose restart [service-name]
   ```

4. **Database Connection Issues**
   ```bash
   # Wait for services to be ready
   docker-compose up -d
   sleep 30
   
   # Test connections
   docker-compose exec postgres pg_isready -U postgres
   ```

---

## 📚 Next Steps

After successful setup:
1. **Task Package 2:** Authentication Service
2. **Task Package 3:** Task Management Service
3. **Task Package 4:** Notes Service

---

## ✅ Acceptance Criteria Met

- [x] All containers start successfully with `docker-compose up`
- [x] Can connect to each database from host machine
- [x] PostgreSQL schemas created and validated
- [x] MongoDB indexes created
- [x] Redis accepts connections
- [x] RabbitMQ management UI accessible
- [x] MinIO buckets created and accessible
- [x] Sample data exists in all databases
- [x] All services communicate properly
- [x] Health checks pass for all services

---

**Task Package 1 Complete!** 🎉
