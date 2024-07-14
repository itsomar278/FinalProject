 # Final Project: Foothill Technology Solutions 

 ## Description 

 This repository contains the final project developed during my internship at Foothill Technology Solutions. It is a .NET 6 Core and ASP.NET 6 Core application implementing an n-layered architecture for robustness and scalability. 

 ## Features 

 - **Authentication:** Secure user authentication using JWT tokens with login/signup pages and a logout feature on the settings page. 
 - **User Management:** Create, read, and update (CRU) user profiles through a settings page. 
 - **Article Management:** Full CRUD functionality for managing articles. 
 - **Comment Management:** Create, read, and delete (CR*D) comments on articles. 
 - **Article Listing:** Display paginated lists of articles. 
 - **Favorites:** Allow users to mark articles as favorites. 
 - **Follow Users:** Enable users to follow other users for updates. 

 ## Technologies Used 

 - **Backend:** .NET 6 Core, ASP.NET 6 Core 
 - **Database:** Entity Framework Core 6 (EFCORE 6), SQL Server 
 - **Authentication:** JWT Authentication with refresh tokens and sessions 
 - **Logging:** Serilog for structured logging to console and files 
 - **Dependency Injection:** Using .NET Core built-in DI container 
 - **API Documentation:** Swagger/OpenAPI for API documentation and testing 
 - **Other:** AutoMapper for object-object mapping, Entity Framework Core for data access 

 ## Getting Started 

 To get started with this project, clone the repository and configure your environment. Make sure you have .NET 6 SDK installed. Update the `appsettings.json` file with your database connection string and security key. 

 1. **Clone the repository:** 
    ```bash 
    git clone [https://github.com/](https://github.com/itsomar278/FinalProject.git)
    cd your-repo 
    ``` 

 2. **Setup Database:** 

 Update appsettings.json with your SQL Server connection string under "ConnectionStrings". 
 Run EF Core migrations to create the database schema: 
    ```bash 
    dotnet ef database update 
    ``` 

 3. **Run the Application:** 

    ```bash 
    dotnet run 
    ``` 

 4. **Explore APIs:** 

 Launch Swagger UI in your browser at https://localhost:{port}/swagger/index.html to explore and test the APIs. 


 ## Acknowledgments 
 - Thanks to Foothill Technology Solutions for the internship opportunity and my Trainer @Sameh Nawasra. 

 ## General Functionality 

 - Authenticate users via JWT . 
 - CRU* users . 
 - CRUD Articles. 
 - CR*D Comments on articles (no updating required). 
 - GET and display paginated lists of articles. 
 - Favorite articles. 
 - Follow other users. 
