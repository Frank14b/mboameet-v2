FROM mcr.microsoft.com/dotnet/sdk:7.0.103

WORKDIR /app

COPY . .

ENV JWT_TOKEN_KEY="$2y$10$h1caL/noWEXVhs9Wri0xN.1/VxEkuancfPbOJ6uMY7USa0dlhAiaC/VxEkuancfPbOJ6uMY7USa0dlhAiaC~"
ENV MONGODB_URI="mongodb://localhost:27017"

EXPOSE 5013 5000

ENTRYPOINT ["dotnet", "run"]

CMD ["dotnet", "watch", "run", "--urls", "http://localhost:5013"]
