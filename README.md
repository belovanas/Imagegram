# Imagegram
## Technologies
* C#
* DynamoDb as database for storing users / posts / comments' info
* S3 as storage system for images
* Lambda for image resizing and converting to jpeg
* Swagger for API documentation

## Architecture
<img width="1102" alt="image" src="https://user-images.githubusercontent.com/22280400/200049744-873912dc-ac26-4a48-81c3-ec4632bac96b.png">


## API
<img width="1337" alt="image" src="https://user-images.githubusercontent.com/22280400/200042438-6240ac49-a7c6-4c54-be98-95dcb1f6988d.png">

## Deploy
Project contains Dockerfile which allows you to create docker image and deploy it on your server.
To do it follow the steps:
1. Build image with command:
```
docker build -t dotnet-docker .
```
2. Copy created image to your server
3. Run docker container with command:
```
docker run --publish 5000:80 dotnet-docker
```
4. Now the API is available on 5000 port. You can use /swagger to find list of methods.
