# **Project: Authentication and API Management System with ASP.NET Core**

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

## **Project Overview**
This project is a web API built using ASP.NET Core that includes all essential features for authentication, user management, token-based security, and API documentation. The key features of this project are:

- **Authentication with ASP.NET Identity:** User and role management using the Identity Framework.
- **JWT Tokens (JSON Web Tokens):** Generation of access tokens and refresh tokens for secure authentication.
- **Refresh Token Management:** Secure storage and validation of refresh tokens in the database.
- **Token Blacklist:** Ability to blacklist invalid or expired tokens.
- **Login and Registration:** Secure user login and registration with strong input validation.
- **API Documentation with Swagger:** Interactive and comprehensive API documentation using Swagger.
- **API Testing with RestSharp:** Example usage of the RestSharp library for testing and consuming the API.

---

## **Key Features**

### 1. **Authentication and User Management**
- Utilizes ASP.NET Identity for user and role management, including password hashing.
- Provides secure login and registration with input validation.

### 2. **JWT Tokens**
- Generates short-lived access tokens for authentication.
- Uses refresh tokens to generate new access tokens without requiring re-login.
- Stores refresh tokens securely in the database.

### 3. **Token Blacklist**
- Allows blacklisting invalid or expired tokens.
- Manages blacklisted tokens by storing them in the database.

### 4. **API Documentation with Swagger**
- Provides interactive and detailed API documentation using Swagger.
- Allows testing of API endpoints directly from the Swagger UI.

### 5. **API Testing with RestSharp**
- Demonstrates how to test and consume the API using the RestSharp library.
- Includes example code snippets for sending HTTP requests and receiving responses.

---

## **Setup Instructions**

### **Prerequisites**
- [.NET SDK](https://dotnet.microsoft.com/download) (minimum version 6)
- SQL Server or any preferred database
- [Postman](https://www.postman.com/) or [RestSharp](https://restsharp.dev/) for API testing

### **Steps to Run the Project**
1. **Clone the Repository:**
   ```bash
   git clone https://github.com/yourusername/MyAuthApi.git
   cd MyAuthApi
Install NuGet Packages:
bash
Copy
1
dotnet restore
Database Configuration:
Open the appsettings.json file and update the database connection string:
json
Copy
1
2
3
⌄
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=YOUR_DB;User Id=YOUR_USER;Password=YOUR_PASSWORD;"
}
Run Database Migrations:
bash
Copy
1
dotnet ef database update
Run the Project:
bash
Copy
1
dotnet run
Access Swagger:
After running the project, you can view the API documentation at:
Copy
1
http://localhost:5000/swagger
API Endpoints
1. User Registration
Endpoint: POST /api/auth/register
Request Body:
json
Copy
1
2
3
4
5
⌄
{
  "username": "string",
  "email": "string",
  "password": "string"
}
Response: Success message or validation error.
2. User Login
Endpoint: POST /api/auth/login
Request Body:
json
Copy
1
2
3
4
⌄
{
  "username": "string",
  "password": "string"
}
Response:
json
Copy
1
2
3
4
⌄
{
  "accessToken": "string",
  "refreshToken": "string"
}
3. Refresh Access Token
Endpoint: POST /api/auth/refresh-token
Request Body:
json
Copy
1
2
3
⌄
{
  "refreshToken": "string"
}
Response:
json
Copy
1
2
3
4
⌄
{
  "accessToken": "string",
  "refreshToken": "string"
}
4. Logout (Blacklist Token)
Endpoint: POST /api/auth/logout
Request Body:
json
Copy
1
2
3
⌄
{
  "refreshToken": "string"
}
Response: Success message or validation error.
Testing the API with RestSharp
You can test the API using RestSharp with the following example code:

csharp
Copy
1
2
3
4
5
6
var client = new RestClient("http://localhost:5000");
var request = new RestRequest("/api/auth/login", Method.Post);
request.AddJsonBody(new { username = "testuser", password = "testpass" });

var response = client.Execute(request);
Console.WriteLine(response.Content);
License
This project is licensed under the MIT License. For more details, see the LICENSE file.

Contributors
Your Name
Contact Us
For questions, feedback, or collaboration, feel free to reach out via email or GitHub Issues:

Email: your.email@example.com
GitHub Issues: Issues
Copy
1
2
3
4

---

Once you’ve crea
