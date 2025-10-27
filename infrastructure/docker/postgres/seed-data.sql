-- PostgreSQL Seed Data for AI Business Management System
-- This script populates the databases with sample data for development

-- Connect to auth_db and insert sample users
\c auth_db;

-- Insert sample users
INSERT INTO users (id, email, username, password_hash, full_name, is_email_verified, is_active) VALUES
    ('550e8400-e29b-41d4-a716-446655440001', 'admin@example.com', 'admin', '$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'System Administrator', true, true),
    ('550e8400-e29b-41d4-a716-446655440002', 'test@example.com', 'testuser', '$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'Test User', true, true),
    ('550e8400-e29b-41d4-a716-446655440003', 'demo@example.com', 'demouser', '$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'Demo User', true, true);

-- Insert user roles
INSERT INTO user_roles (user_id, role) VALUES
    ('550e8400-e29b-41d4-a716-446655440001', 'Admin'),
    ('550e8400-e29b-41d4-a716-446655440001', 'User'),
    ('550e8400-e29b-41d4-a716-446655440002', 'User'),
    ('550e8400-e29b-41d4-a716-446655440003', 'User');

-- Insert sample API keys
INSERT INTO api_keys (user_id, key_name, key_hash, prefix, scopes, expires_at) VALUES
    ('550e8400-e29b-41d4-a716-446655440001', 'Development Key', '$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'dev_', ARRAY['read', 'write'], NOW() + INTERVAL '1 year'),
    ('550e8400-e29b-41d4-a716-446655440002', 'Test Key', '$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'test_', ARRAY['read'], NOW() + INTERVAL '6 months');

-- Connect to taskmanager_db and insert sample tasks
\c taskmanager_db;

-- Insert sample tasks
INSERT INTO tasks (id, user_id, title, description, status, priority, priority_order, due_date, estimated_duration_minutes) VALUES
    ('660e8400-e29b-41d4-a716-446655440001', '550e8400-e29b-41d4-a716-446655440001', 'Set up database infrastructure', 'Complete Task Package 1: Database Infrastructure Setup', 'InProgress', 'A', 1, NOW() + INTERVAL '2 days', 480),
    ('660e8400-e29b-41d4-a716-446655440002', '550e8400-e29b-41d4-a716-446655440001', 'Create authentication service', 'Build JWT-based authentication with user management', 'NotStarted', 'A', 2, NOW() + INTERVAL '5 days', 600),
    ('660e8400-e29b-41d4-a716-446655440003', '550e8400-e29b-41d4-a716-446655440001', 'Implement task management', 'Build core task CRUD operations with ABC prioritization', 'NotStarted', 'B', 1, NOW() + INTERVAL '10 days', 800),
    ('660e8400-e29b-41d4-a716-446655440004', '550e8400-e29b-41d4-a716-446655440002', 'Review documentation', 'Read through all project documentation', 'Completed', 'B', 2, NOW() - INTERVAL '1 day', 120),
    ('660e8400-e29b-41d4-a716-446655440005', '550e8400-e29b-41d4-a716-446655440002', 'Set up development environment', 'Install Docker, .NET SDK, and configure IDE', 'Completed', 'C', 1, NOW() - INTERVAL '2 days', 180),
    ('660e8400-e29b-41d4-a716-446655440006', '550e8400-e29b-41d4-a716-446655440003', 'Learn .NET MAUI', 'Study mobile app development with .NET MAUI', 'NotStarted', 'C', 2, NOW() + INTERVAL '30 days', 1200);

-- Insert sample task tags
INSERT INTO task_tags (task_id, tag) VALUES
    ('660e8400-e29b-41d4-a716-446655440001', 'infrastructure'),
    ('660e8400-e29b-41d4-a716-446655440001', 'database'),
    ('660e8400-e29b-41d4-a716-446655440001', 'docker'),
    ('660e8400-e29b-41d4-a716-446655440002', 'authentication'),
    ('660e8400-e29b-41d4-a716-446655440002', 'security'),
    ('660e8400-e29b-41d4-a716-446655440002', 'jwt'),
    ('660e8400-e29b-41d4-a716-446655440003', 'tasks'),
    ('660e8400-e29b-41d4-a716-446655440003', 'crud'),
    ('660e8400-e29b-41d4-a716-446655440003', 'prioritization'),
    ('660e8400-e29b-41d4-a716-446655440004', 'documentation'),
    ('660e8400-e29b-41d4-a716-446655440004', 'review'),
    ('660e8400-e29b-41d4-a716-446655440005', 'setup'),
    ('660e8400-e29b-41d4-a716-446655440005', 'environment'),
    ('660e8400-e29b-41d4-a716-446655440006', 'mobile'),
    ('660e8400-e29b-41d4-a716-446655440006', 'learning'),
    ('660e8400-e29b-41d4-a716-446655440006', 'maui');

-- Insert sample task categories
INSERT INTO task_categories (task_id, category) VALUES
    ('660e8400-e29b-41d4-a716-446655440001', 'Development'),
    ('660e8400-e29b-41d4-a716-446655440001', 'Infrastructure'),
    ('660e8400-e29b-41d4-a716-446655440002', 'Development'),
    ('660e8400-e29b-41d4-a716-446655440002', 'Backend'),
    ('660e8400-e29b-41d4-a716-446655440003', 'Development'),
    ('660e8400-e29b-41d4-a716-446655440003', 'Backend'),
    ('660e8400-e29b-41d4-a716-446655440004', 'Planning'),
    ('660e8400-e29b-41d4-a716-446655440005', 'Setup'),
    ('660e8400-e29b-41d4-a716-446655440006', 'Learning'),
    ('660e8400-e29b-41d4-a716-446655440006', 'Mobile');

-- Insert sample task dependencies
INSERT INTO task_dependencies (task_id, depends_on_task_id, dependency_type) VALUES
    ('660e8400-e29b-41d4-a716-446655440002', '660e8400-e29b-41d4-a716-446655440001', 'FinishToStart'),
    ('660e8400-e29b-41d4-a716-446655440003', '660e8400-e29b-41d4-a716-446655440002', 'FinishToStart');

-- Insert sample task reminders
INSERT INTO task_reminders (task_id, reminder_type, reminder_time, delivery_channels) VALUES
    ('660e8400-e29b-41d4-a716-446655440001', 'BeforeDueDate', NOW() + INTERVAL '1 day', ARRAY['email', 'push']),
    ('660e8400-e29b-41d4-a716-446655440002', 'OneTime', NOW() + INTERVAL '3 days', ARRAY['email']),
    ('660e8400-e29b-41d4-a716-446655440003', 'BeforeDueDate', NOW() + INTERVAL '7 days', ARRAY['push']);

-- Insert sample task templates
INSERT INTO task_templates (user_id, name, description, template_data, is_shared) VALUES
    ('550e8400-e29b-41d4-a716-446655440001', 'Development Task', 'Template for development tasks', '{"priority": "B", "tags": ["development"], "categories": ["Development"], "estimated_duration_minutes": 240}', true),
    ('550e8400-e29b-41d4-a716-446655440001', 'Bug Fix', 'Template for bug fixes', '{"priority": "A", "tags": ["bug", "fix"], "categories": ["Development"], "estimated_duration_minutes": 120}', true),
    ('550e8400-e29b-41d4-a716-446655440002', 'Learning Task', 'Template for learning activities', '{"priority": "C", "tags": ["learning"], "categories": ["Learning"], "estimated_duration_minutes": 180}', false);

-- Connect to agentqueue_db and insert sample agent tasks
\c agentqueue_db;

-- Insert sample agent tasks
INSERT INTO agent_tasks (id, user_id, task_type, task_parameters, priority, status, progress_percentage, progress_message) VALUES
    ('770e8400-e29b-41d4-a716-446655440001', '550e8400-e29b-41d4-a716-446655440001', 'Research', '{"topic": "AI Agent Business Management Systems", "depth": "comprehensive", "outputFormat": "markdown"}', 'High', 'Completed', 100, 'Research completed successfully'),
    ('770e8400-e29b-41d4-a716-446655440002', '550e8400-e29b-41d4-a716-446655440001', 'ContentGeneration', '{"contentType": "blog", "prompt": "Write a blog post about microservices architecture", "length": "medium", "tone": "professional"}', 'Normal', 'InProgress', 65, 'Generating content...'),
    ('770e8400-e29b-41d4-a716-446655440003', '550e8400-e29b-41d4-a716-446655440002', 'DataAnalysis', '{"dataSource": "task_completion_metrics", "analysisType": "trend", "outputFormat": "report"}', 'Low', 'Queued', 0, 'Waiting in queue'),
    ('770e8400-e29b-41d4-a716-446655440004', '550e8400-e29b-41d4-a716-446655440003', 'Summarization', '{"documentType": "meeting_notes", "documentId": "meeting_2025_10_26", "summaryLength": "brief"}', 'Normal', 'Failed', 0, 'Document not found');

-- Insert sample agent task logs
INSERT INTO agent_task_logs (task_id, log_level, message, metadata) VALUES
    ('770e8400-e29b-41d4-a716-446655440001', 'INFO', 'Research task started', '{"startTime": "2025-10-26T10:00:00Z"}'),
    ('770e8400-e29b-41d4-a716-446655440001', 'INFO', 'Gathering sources from web', '{"sourcesFound": 15}'),
    ('770e8400-e29b-41d4-a716-446655440001', 'INFO', 'Analyzing and summarizing content', '{"processingTime": "00:05:30"}'),
    ('770e8400-e29b-41d4-a716-446655440001', 'INFO', 'Research task completed successfully', '{"endTime": "2025-10-26T10:15:00Z", "resultSize": "2048"}'),
    ('770e8400-e29b-41d4-a716-446655440002', 'INFO', 'Content generation started', '{"startTime": "2025-10-26T11:00:00Z"}'),
    ('770e8400-e29b-41d4-a716-446655440002', 'INFO', 'Analyzing prompt and requirements', '{"promptLength": 85}'),
    ('770e8400-e29b-41d4-a716-446655440002', 'INFO', 'Generating content structure', '{"sections": 5}'),
    ('770e8400-e29b-41d4-a716-446655440003', 'INFO', 'Task queued for processing', '{"queuePosition": 1}'),
    ('770e8400-e29b-41d4-a716-446655440004', 'ERROR', 'Document not found', '{"documentId": "meeting_2025_10_26", "errorCode": "DOC_NOT_FOUND"}');

-- Update completed task timestamps
UPDATE agent_tasks SET 
    started_at = NOW() - INTERVAL '1 hour',
    completed_at = NOW() - INTERVAL '45 minutes'
WHERE id = '770e8400-e29b-41d4-a716-446655440001';

UPDATE agent_tasks SET 
    started_at = NOW() - INTERVAL '30 minutes'
WHERE id = '770e8400-e29b-41d4-a716-446655440002';

-- Update completed task timestamps in taskmanager_db
\c taskmanager_db;
UPDATE tasks SET 
    completed_at = NOW() - INTERVAL '1 day',
    actual_duration_minutes = 90
WHERE id = '660e8400-e29b-41d4-a716-446655440004';

UPDATE tasks SET 
    completed_at = NOW() - INTERVAL '2 days',
    actual_duration_minutes = 150
WHERE id = '660e8400-e29b-41d4-a716-446655440005';
