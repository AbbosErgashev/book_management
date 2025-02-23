This API is designed to handle the management of books, including user authentication, book CRUD operations (create, read, update, delete), and handling popular books. Additionally, there is a security layer where JWT (JSON Web Token) authentication is used for both users and admins.

**_Key Features:_**
User Authentication: Admin and users can log in and register.

Admin Credentials: The system has a hardcoded admin account with the following credentials:
**Email: admin**
**Password: password**

Users can authenticate via JWT tokens. Upon successful login, a JWT token is returned for the user to use for subsequent requests.

**_Books Management:_**
Add a Book: Users can add a new book to the system.
View a Book: The details of a book can be retrieved.
Update a Book: Users can update book details.
Delete a Book: Soft delete functionality is available to remove books from the system without permanently erasing them.
Popular Books: The system tracks and displays popular books based on the number of views.

**_Authentication and Authorization_**
JWT Authentication is used to secure the endpoints:
A user must include a valid JWT token in the Authorization header to access protected resources.
The Admin role is distinguished by the credentials mentioned above.

**_Swagger Documentation_**
The API includes Swagger UI for interactive API documentation. It allows you to test API calls directly from the browser.
Swagger requires an Authorization token to test any protected endpoints. This token should be prefixed by Bearer, e.g., 
Authorization: **Bearer {your-token-here}.**

**_Error Handling_**
The system has middleware to catch exceptions and log errors appropriately. This ensures better traceability and debugging during development or in case of failures.

_**Endpoints Overview**_
_User Registration:_
**POST** /api/auth/signup: Registers a new user.
**User Login** (Admin or Regular User):
**POST** /api/auth/signin: Authenticates the user or admin and returns a JWT token.

_Book Operations:_
**POST** /api/books: Add a new book.

**GET** /api/books/{id}: Retrieve details of a book.

**PUT** /api/books/{id}: Update book information.

**DELETE** /api/books/{id}: Soft delete a book.

**GET** /api/books/popular: Retrieve popular books based on views.
