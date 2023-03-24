FROM mcr.microsoft.com/dotnet/sdk:7.0 as base
RUN apt-get update && \
    apt-get install -yq ssh git

FROM base as hot
# Setup git SSH
ARG PORTAINER_SSH_KEY
RUN mkdir /root/.ssh/ && \
    echo $PORTAINER_SSH_KEY | base64 --decode >> /root/.ssh/docker_key && \
    chmod 400 /root/.ssh/docker_key && \
    touch /root/.ssh/known_hosts && \
    ssh-keyscan github.com >> /root/.ssh/known_hosts
RUN git config --global user.email "temp@temp.com" && git config --global user.name "Portainer"

# Clone solution from github
WORKDIR /src
RUN eval $(ssh-agent) && ssh-add /root/.ssh/docker_key && git clone --branch main git@github.com:goffincedric/MusicDownloaderAPI.git
WORKDIR MusicDownloaderAPI
# Restore packages in advance for faster updates
RUN dotnet restore "MusicDownloader/MusicDownloader.csproj"

FROM hot as final
WORKDIR /src/MusicDownloaderAPI
# Expose ports and run server
EXPOSE 80
# Set environment variables
ENV ASPNETCORE_URLS=http://+:80

CMD ./MusicDownloader/scripts/update.sh && ./MusicDownloader/scripts/publish.sh && ./MusicDownloader/scripts/run.sh