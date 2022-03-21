# boulderingtimesui

## Project setup
```
npm install
```

### Compiles and hot-reloads for development
```
npm run serve
```

### Compiles and minifies for production
```
npm run build
```

### Lints and fixes files
```
npm run lint
```

### Customize configuration
See [Configuration Reference](https://cli.vuejs.org/config/).

# BoulderingTimes.Api
1. cd BoulderingTimes.Api
1. dotnet build
1. dotnet run

# Usage of docker
1. Install docker and docker-compose (both available in apt-get)
1. Make sure to be in the root folder
1. `docker-compose build`
1. `docker-compose up`

## push with docker-compose
1. Make sure you have a dockerhub account and are logged in
To login simply put `docker login`
1. Make sure the compose is build or build with `docker-compose build`
1. `docker-compose push`