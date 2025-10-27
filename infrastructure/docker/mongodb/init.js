// MongoDB Initialization Script for AI Business Management System
// This script creates collections and indexes for the notes service

// Switch to notes_db
db = db.getSiblingDB('notes_db');

// Create notes collection with validation
db.createCollection('notes', {
    validator: {
        $jsonSchema: {
            bsonType: 'object',
            required: ['userId', 'content', 'createdAt', 'modifiedAt'],
            properties: {
                userId: {
                    bsonType: 'string',
                    description: 'User ID must be a string and is required'
                },
                notebookId: {
                    bsonType: 'string',
                    description: 'Notebook ID must be a string'
                },
                title: {
                    bsonType: 'string',
                    maxLength: 200,
                    description: 'Title must be a string with max 200 characters'
                },
                content: {
                    bsonType: 'string',
                    maxLength: 100000,
                    description: 'Content must be a string with max 100,000 characters'
                },
                contentType: {
                    bsonType: 'string',
                    enum: ['markdown', 'richtext'],
                    description: 'Content type must be markdown or richtext'
                },
                noteType: {
                    bsonType: 'string',
                    enum: ['Idea', 'MeetingNotes', 'QuickNote', 'Journal', 'Research', 'Reference'],
                    description: 'Note type must be one of the predefined types'
                },
                tags: {
                    bsonType: 'array',
                    items: {
                        bsonType: 'string',
                        maxLength: 50
                    },
                    description: 'Tags must be an array of strings with max 50 characters each'
                },
                color: {
                    bsonType: 'string',
                    description: 'Color must be a string'
                },
                isFavorite: {
                    bsonType: 'bool',
                    description: 'isFavorite must be a boolean'
                },
                isArchived: {
                    bsonType: 'bool',
                    description: 'isArchived must be a boolean'
                },
                isPinned: {
                    bsonType: 'bool',
                    description: 'isPinned must be a boolean'
                },
                location: {
                    bsonType: 'object',
                    properties: {
                        latitude: { bsonType: 'double' },
                        longitude: { bsonType: 'double' },
                        address: { bsonType: 'string' }
                    },
                    description: 'Location must be an object with latitude, longitude, and address'
                },
                attachments: {
                    bsonType: 'array',
                    items: {
                        bsonType: 'object',
                        required: ['id', 'fileName', 'filePath', 'fileSize', 'contentType', 'uploadedAt'],
                        properties: {
                            id: { bsonType: 'string' },
                            fileName: { bsonType: 'string' },
                            filePath: { bsonType: 'string' },
                            fileSize: { bsonType: 'number' },
                            contentType: { bsonType: 'string' },
                            thumbnailPath: { bsonType: 'string' },
                            extractedText: { bsonType: 'string' },
                            uploadedAt: { bsonType: 'date' }
                        }
                    },
                    description: 'Attachments must be an array of attachment objects'
                },
                checklists: {
                    bsonType: 'array',
                    items: {
                        bsonType: 'object',
                        required: ['id', 'text', 'isChecked', 'order'],
                        properties: {
                            id: { bsonType: 'string' },
                            text: { bsonType: 'string' },
                            isChecked: { bsonType: 'bool' },
                            order: { bsonType: 'number' }
                        }
                    },
                    description: 'Checklists must be an array of checklist item objects'
                },
                metadata: {
                    bsonType: 'object',
                    properties: {
                        wordCount: { bsonType: 'number' },
                        characterCount: { bsonType: 'number' },
                        readingTimeMinutes: { bsonType: 'number' }
                    },
                    description: 'Metadata must be an object with word count, character count, and reading time'
                },
                createdAt: {
                    bsonType: 'date',
                    description: 'Created date must be a date and is required'
                },
                modifiedAt: {
                    bsonType: 'date',
                    description: 'Modified date must be a date and is required'
                },
                lastAccessedAt: {
                    bsonType: 'date',
                    description: 'Last accessed date must be a date'
                },
                isDeleted: {
                    bsonType: 'bool',
                    description: 'isDeleted must be a boolean'
                },
                deletedAt: {
                    bsonType: 'date',
                    description: 'Deleted date must be a date'
                }
            }
        }
    }
});

// Create notebooks collection
db.createCollection('notebooks', {
    validator: {
        $jsonSchema: {
            bsonType: 'object',
            required: ['userId', 'name', 'createdAt', 'modifiedAt'],
            properties: {
                userId: {
                    bsonType: 'string',
                    description: 'User ID must be a string and is required'
                },
                name: {
                    bsonType: 'string',
                    maxLength: 100,
                    description: 'Name must be a string with max 100 characters'
                },
                description: {
                    bsonType: 'string',
                    maxLength: 500,
                    description: 'Description must be a string with max 500 characters'
                },
                color: {
                    bsonType: 'string',
                    description: 'Color must be a string'
                },
                icon: {
                    bsonType: 'string',
                    description: 'Icon must be a string'
                },
                parentNotebookId: {
                    bsonType: 'string',
                    description: 'Parent notebook ID must be a string'
                },
                order: {
                    bsonType: 'number',
                    description: 'Order must be a number'
                },
                createdAt: {
                    bsonType: 'date',
                    description: 'Created date must be a date and is required'
                },
                modifiedAt: {
                    bsonType: 'date',
                    description: 'Modified date must be a date and is required'
                },
                isDeleted: {
                    bsonType: 'bool',
                    description: 'isDeleted must be a boolean'
                }
            }
        }
    }
});

// Create note versions collection for history tracking
db.createCollection('note_versions', {
    validator: {
        $jsonSchema: {
            bsonType: 'object',
            required: ['noteId', 'userId', 'content', 'versionNumber', 'createdAt', 'createdBy'],
            properties: {
                noteId: {
                    bsonType: 'string',
                    description: 'Note ID must be a string and is required'
                },
                userId: {
                    bsonType: 'string',
                    description: 'User ID must be a string and is required'
                },
                content: {
                    bsonType: 'string',
                    description: 'Content must be a string and is required'
                },
                versionNumber: {
                    bsonType: 'number',
                    description: 'Version number must be a number and is required'
                },
                createdAt: {
                    bsonType: 'date',
                    description: 'Created date must be a date and is required'
                },
                createdBy: {
                    bsonType: 'string',
                    description: 'Created by must be a string and is required'
                },
                changeDescription: {
                    bsonType: 'string',
                    description: 'Change description must be a string'
                }
            }
        }
    }
});

// Create indexes for notes collection
db.notes.createIndex({ userId: 1, isDeleted: 1, modifiedAt: -1 });
db.notes.createIndex({ userId: 1, tags: 1 });
db.notes.createIndex({ userId: 1, notebookId: 1 });
db.notes.createIndex({ userId: 1, noteType: 1 });
db.notes.createIndex({ userId: 1, isFavorite: 1 });
db.notes.createIndex({ userId: 1, isArchived: 1 });
db.notes.createIndex({ userId: 1, isPinned: 1 });
db.notes.createIndex({ createdAt: -1 });
db.notes.createIndex({ modifiedAt: -1 });
db.notes.createIndex({ lastAccessedAt: -1 });

// Create full-text search index
db.notes.createIndex({
    title: 'text',
    content: 'text',
    'attachments.extractedText': 'text'
}, {
    name: 'notes_fulltext',
    weights: {
        title: 10,
        content: 5,
        'attachments.extractedText': 1
    }
});

// Create indexes for notebooks collection
db.notebooks.createIndex({ userId: 1, isDeleted: 1 });
db.notebooks.createIndex({ userId: 1, parentNotebookId: 1 });
db.notebooks.createIndex({ userId: 1, order: 1 });

// Create indexes for note versions collection
db.note_versions.createIndex({ noteId: 1, versionNumber: -1 });
db.note_versions.createIndex({ userId: 1, createdAt: -1 });

// Insert sample notebooks
db.notebooks.insertMany([
    {
        _id: ObjectId(),
        userId: '550e8400-e29b-41d4-a716-446655440001',
        name: 'Project Planning',
        description: 'Notes related to project planning and management',
        color: '#3B82F6',
        icon: '📋',
        order: 1,
        createdAt: new Date(),
        modifiedAt: new Date(),
        isDeleted: false
    },
    {
        _id: ObjectId(),
        userId: '550e8400-e29b-41d4-a716-446655440001',
        name: 'Development Ideas',
        description: 'Ideas and concepts for development',
        color: '#10B981',
        icon: '💡',
        order: 2,
        createdAt: new Date(),
        modifiedAt: new Date(),
        isDeleted: false
    },
    {
        _id: ObjectId(),
        userId: '550e8400-e29b-41d4-a716-446655440002',
        name: 'Learning Notes',
        description: 'Notes from learning and research',
        color: '#F59E0B',
        icon: '📚',
        order: 1,
        createdAt: new Date(),
        modifiedAt: new Date(),
        isDeleted: false
    }
]);

// Insert sample notes
db.notes.insertMany([
    {
        _id: ObjectId(),
        userId: '550e8400-e29b-41d4-a716-446655440001',
        notebookId: db.notebooks.findOne({name: 'Project Planning'})._id.toString(),
        title: 'Database Architecture Decisions',
        content: '# Database Architecture Decisions\n\n## PostgreSQL Setup\n- Using PostgreSQL 16 for relational data\n- Separate databases for different services\n- Proper indexing for performance\n\n## MongoDB Setup\n- Using MongoDB 7 for document storage\n- Flexible schema for notes and attachments\n- Full-text search capabilities\n\n## Redis Setup\n- Caching layer for performance\n- Session management\n- Real-time features support',
        contentType: 'markdown',
        noteType: 'Research',
        tags: ['database', 'architecture', 'postgresql', 'mongodb', 'redis'],
        color: '#3B82F6',
        isFavorite: true,
        isArchived: false,
        isPinned: true,
        attachments: [],
        checklists: [
            {
                id: ObjectId().toString(),
                text: 'Set up PostgreSQL containers',
                isChecked: true,
                order: 1
            },
            {
                id: ObjectId().toString(),
                text: 'Configure MongoDB collections',
                isChecked: true,
                order: 2
            },
            {
                id: ObjectId().toString(),
                text: 'Set up Redis caching',
                isChecked: false,
                order: 3
            }
        ],
        metadata: {
            wordCount: 45,
            characterCount: 320,
            readingTimeMinutes: 1
        },
        createdAt: new Date(),
        modifiedAt: new Date(),
        lastAccessedAt: new Date(),
        isDeleted: false
    },
    {
        _id: ObjectId(),
        userId: '550e8400-e29b-41d4-a716-446655440001',
        notebookId: db.notebooks.findOne({name: 'Development Ideas'})._id.toString(),
        title: 'AI Agent Integration Ideas',
        content: '## AI Agent Integration Ideas\n\n### MCP Server Implementation\n- Model Context Protocol for AI agent communication\n- Tool definitions for task management\n- Real-time progress tracking\n\n### Agent Task Types\n- Research tasks with web search\n- Content generation\n- Data analysis and reporting\n- Document summarization\n\n### Future Enhancements\n- Voice-to-text integration\n- Image OCR capabilities\n- Smart scheduling algorithms',
        contentType: 'markdown',
        noteType: 'Idea',
        tags: ['ai', 'agents', 'mcp', 'integration', 'future'],
        color: '#10B981',
        isFavorite: false,
        isArchived: false,
        isPinned: false,
        attachments: [],
        checklists: [
            {
                id: ObjectId().toString(),
                text: 'Research MCP protocol',
                isChecked: true,
                order: 1
            },
            {
                id: ObjectId().toString(),
                text: 'Design agent task types',
                isChecked: true,
                order: 2
            },
            {
                id: ObjectId().toString(),
                text: 'Implement MCP servers',
                isChecked: false,
                order: 3
            }
        ],
        metadata: {
            wordCount: 38,
            characterCount: 280,
            readingTimeMinutes: 1
        },
        createdAt: new Date(),
        modifiedAt: new Date(),
        lastAccessedAt: new Date(),
        isDeleted: false
    },
    {
        _id: ObjectId(),
        userId: '550e8400-e29b-41d4-a716-446655440002',
        notebookId: db.notebooks.findOne({name: 'Learning Notes'})._id.toString(),
        title: 'Microservices Best Practices',
        content: '## Microservices Best Practices\n\n### Service Design\n- Single responsibility principle\n- Database per service\n- API-first design\n\n### Communication\n- RESTful APIs\n- Message queues for async operations\n- Circuit breaker pattern\n\n### Deployment\n- Containerization with Docker\n- Kubernetes orchestration\n- CI/CD pipelines\n\n### Monitoring\n- Distributed tracing\n- Centralized logging\n- Health checks',
        contentType: 'markdown',
        noteType: 'Research',
        tags: ['microservices', 'architecture', 'best-practices', 'learning'],
        color: '#F59E0B',
        isFavorite: true,
        isArchived: false,
        isPinned: false,
        attachments: [],
        checklists: [
            {
                id: ObjectId().toString(),
                text: 'Study service design patterns',
                isChecked: true,
                order: 1
            },
            {
                id: ObjectId().toString(),
                text: 'Learn about API design',
                isChecked: true,
                order: 2
            },
            {
                id: ObjectId().toString(),
                text: 'Practice with Docker',
                isChecked: false,
                order: 3
            }
        ],
        metadata: {
            wordCount: 42,
            characterCount: 310,
            readingTimeMinutes: 1
        },
        createdAt: new Date(),
        modifiedAt: new Date(),
        lastAccessedAt: new Date(),
        isDeleted: false
    }
]);

print('MongoDB initialization completed successfully!');
print('Created collections: notes, notebooks, note_versions');
print('Created indexes for performance optimization');
print('Inserted sample data for development');
