#!/bin/bash
# MinIO Bucket Setup Script for AI Business Management System
# This script creates the required buckets and sets up access policies

set -e

# Configuration
MINIO_ENDPOINT="http://localhost:9000"
MINIO_ACCESS_KEY="minioadmin"
MINIO_SECRET_KEY="minioadmin123"
MINIO_MC_ALIAS="ai-business"

# Buckets to create
BUCKETS=(
    "tasks-attachments"
    "notes-attachments"
    "agent-results"
    "user-avatars"
    "temp-uploads"
)

echo "Setting up MinIO buckets for AI Business Management System..."

# Install MinIO client if not present
if ! command -v mc &> /dev/null; then
    echo "Installing MinIO client..."
    if [[ "$OSTYPE" == "linux-gnu"* ]]; then
        wget https://dl.min.io/client/mc/release/linux-amd64/mc -O /usr/local/bin/mc
        chmod +x /usr/local/bin/mc
    elif [[ "$OSTYPE" == "darwin"* ]]; then
        brew install minio/stable/mc
    else
        echo "Please install MinIO client manually: https://docs.min.io/docs/minio-client-quickstart-guide.html"
        exit 1
    fi
fi

# Configure MinIO client
echo "Configuring MinIO client..."
mc alias set $MINIO_MC_ALIAS $MINIO_ENDPOINT $MINIO_ACCESS_KEY $MINIO_SECRET_KEY

# Wait for MinIO to be ready
echo "Waiting for MinIO to be ready..."
for i in {1..30}; do
    if mc admin info $MINIO_MC_ALIAS &> /dev/null; then
        echo "MinIO is ready!"
        break
    fi
    echo "Waiting for MinIO... ($i/30)"
    sleep 2
done

# Create buckets
echo "Creating buckets..."
for bucket in "${BUCKETS[@]}"; do
    echo "Creating bucket: $bucket"
    mc mb $MINIO_MC_ALIAS/$bucket --ignore-existing
    
    # Set bucket policy for public read (for development)
    mc anonymous set public $MINIO_MC_ALIAS/$bucket
    
    # Set lifecycle policy for temp-uploads bucket
    if [ "$bucket" = "temp-uploads" ]; then
        echo "Setting lifecycle policy for temp-uploads bucket..."
        cat > /tmp/lifecycle.json << EOF
{
    "Rules": [
        {
            "ID": "DeleteTempFiles",
            "Status": "Enabled",
            "Filter": {
                "Prefix": ""
            },
            "Expiration": {
                "Days": 7
            }
        }
    ]
}
EOF
        mc ilm import $MINIO_MC_ALIAS/$bucket < /tmp/lifecycle.json
        rm /tmp/lifecycle.json
    fi
done

# Create a test file to verify setup
echo "Creating test file..."
echo "MinIO setup completed successfully!" > /tmp/test.txt
mc cp /tmp/test.txt $MINIO_MC_ALIAS/temp-uploads/test.txt
rm /tmp/test.txt

# Display bucket information
echo ""
echo "MinIO setup completed successfully!"
echo "Buckets created:"
mc ls $MINIO_MC_ALIAS

echo ""
echo "Access URLs:"
echo "MinIO Console: http://localhost:9001"
echo "MinIO API: http://localhost:9000"
echo "Credentials: minioadmin / minioadmin123"

echo ""
echo "Bucket policies:"
for bucket in "${BUCKETS[@]}"; do
    echo "Bucket: $bucket"
    mc anonymous get $MINIO_MC_ALIAS/$bucket
done
