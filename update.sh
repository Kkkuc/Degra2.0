#!/bin/bash
echo "Pulling latest changes from Git..."
git pull origin main

echo "Rebuilding and restarting Docker containers..."
sudo docker compose up -d --build

echo "Applying any new EF Core database migrations..."

cd /var/www/Degra2.0/WebApplication
dotnet ef database update --connection 'User Id=system;Password=system_password;Data Source=localhost:1521/FREEPDB1;'

echo "Deployment complete!"
