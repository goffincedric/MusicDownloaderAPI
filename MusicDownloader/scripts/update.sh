#!/bin/bash
cd /src/MusicDownloaderAPI
# Pull latest changes from github
eval $(ssh-agent) && ssh-add /root/.ssh/docker_key && git fetch --all && git reset --hard origin/main