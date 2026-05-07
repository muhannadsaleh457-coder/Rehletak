# Rehletak Authentication API Documentation
## Frontend Integration Guide

**Base URL:** `https://your-api-domain.com/api`

---

## Table of Contents
1. [SMS OTP Authentication](#sms-otp-authentication)
2. [Email Registration](#email-registration)
3. [Login](#login)
4. [Google OAuth Login](#google-oauth-login)
5. [Password Reset](#password-reset)
6. [Token Refresh](#token-refresh)
7. [User Profile](#user-profile)
8. [Error Handling](#error-handling)

---

## SMS OTP Authentication

### 1. Send SMS OTP

**Endpoint:** `POST /Auth/send-sms-otp`

**Description:** Sends a 6-digit OTP to the provided phone number via SMS.

**Request Body:**
```json
{
  "phoneNumper": "+20123456789"
}
```

**Response (Success - 200 OK):**
```json
{
  "message": "OTP sent successfully",
  "expiers_in": 120
}
```

**Response (Error - 400/500):**
```json
{
  "message": "Error message describing the issue"
}
```

**Notes:**
- OTP is valid for 2 minutes (120 seconds)
- Phone number must include country code
- OTP is a 6-digit random number

---

### 2. Verify SMS OTP

**Endpoint:** `POST /Auth/verify-sms-otp`

**Description:** Verifies the OTP sent to the phone number. Returns different responses for new and existing users.

**Request Body:**
```json
{
  "phoneNumper": "+20123456789",
  "otp": "123456"
}
```

**Response for Existing User (200 OK):**
```json
{
  "is_new_user": false,
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "dummy_refresh",
  "user": {
    "id": "user-uuid",
    "userName": "john_doe",
    "role": "user",
    "token": ""
  }
}
```

**Response for New User (200 OK):**
```json
{
  "is_new_user": true,
  "tempToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**Response (Error - 401):**
```json
{
  "message": "Invalid or expired OTP"
}
```

**Frontend Flow:**
- If `is_new_user` is `true`, redirect to complete registration page
- If `is_new_user` is `false`, store `accessToken` and proceed to dashboard
- Save `tempToken` for new users to use in registration completion

---

## Email Registration

### 1. Register by Email

**Endpoint:** `POST /Auth/register`

**Description:** Initiates email-based registration by sending an OTP to the provided email address.

**Request Body:**
```json
{
  "fullName": "John Doe",
  "phoneNumber": "+20123456789",
  "email": "john@example.com",
  "password": "SecurePassword123!"
}
```

**Response (Success - 200 OK):**
```json
{
  "message": "OTP sent successfully to your email"
}
```

**Response (Error - 400):**
```json
{
  "message": "User with this email already exists"
}
```

**Response (Error - 500):**
```json
{
  "message": "Error during registration: [specific error]"
}
```

**Notes:**
- Password requirements: At least 8 characters, mix of uppercase, lowercase, numbers, and special characters
- OTP is sent to the email address and is valid for 2 minutes
- Email must be unique in the system

---

### 2. Verify Email OTP

**Endpoint:** `POST /Auth/VerifyEmailOtp`

**Description:** Verifies the OTP sent to email and creates the user account.

**Request Body:**
```json
{
  "email": "john@example.com",
  "otp": "123456"
}
```

**Response (Success - 200 OK):**
```json
{
  "is_new_user": true,
  "temp_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**Response (Error - 401):**
```json
{
  "message": "Invalid or expired OTP"
}
```

**Response (Error - 500):**
```json
{
  "message": "Error during email verification: [specific error]"
}
```

**Frontend Flow:**
1. User enters email and password
2. Call `/register` endpoint
3. User receives OTP in email
4. User enters OTP in verification form
5. Call `/VerifyEmailOtp` endpoint
6. Store `temp_token` and redirect to profile completion page if needed
7. Use token for authenticated requests

---

## Login

### Login with Email and Password

**Endpoint:** `POST /Auth/login`

**Description:** Authenticates user with email and password credentials.

**Request Body:**
```json
{
  "email": "john@example.com",
  "password": "SecurePassword123!"
}
```

**Response (Success - 200 OK):**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "refresh_token_value",
  "user": {
    "id": "user-uuid",
    "userName": "john_doe",
    "role": "user",
    "token": ""
  }
}
```

**Response (Error - 401):**
```json
{
  "message": "Invalid email or password"
}
```

**Frontend Implementation:**
```javascript
async function login(email, password) {
  const response = await fetch('https://api.domain.com/api/Auth/login', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({ email, password })
  });
  
  const data = await response.json();
  if (response.ok) {
    localStorage.setItem('accessToken', data.accessToken);
    localStorage.setItem('refreshToken', data.refreshToken);
    localStorage.setItem('user', JSON.stringify(data.user));
  }
  return data;
}
```

---

## Google OAuth Login

### 1. Initiate Google Login

**Endpoint:** `GET /Auth/google/login`

**Description:** Redirects user to Google OAuth consent screen for authentication.

**Response:**
- Redirects to Google OAuth page
- User logs in with their Google account
- Automatically redirects to `/Auth/google/handle` after successful authentication

**Frontend Implementation:**
```javascript
function initiateGoogleLogin() {
  // Redirect to your backend's Google login endpoint
  window.location.href = 'https://your-api-domain.com/api/Auth/google/login';
}
```

---

### 2. Handle Google Login Callback

**Endpoint:** `GET /Auth/google/handle`

**Description:** Handles the OAuth callback from Google. Creates a new user if they don't exist, or logs in existing user. Returns JWT tokens.

**Response (Success - 200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "refresh_token_value"
}
```

**Response (Error - 401):**
```json
{
  "message": "Unauthorized - Failed to authenticate with Google"
}
```

**Response (Error - 500):**
```json
{
  "message": "User creation failed"
}
```

**Frontend Flow:**
1. User clicks "Login with Google" button
2. Call `GET /Auth/google/login` (or set href to this endpoint)
3. User is redirected to Google consent screen
4. After authorization, backend automatically redirects back to `/Auth/google/handle`
5. Backend processes OAuth token and returns JWT tokens
6. Frontend receives tokens in response
7. Store `token` as access token and `refreshToken` for future requests

**Features:**
- Automatically creates new user account if Google email is not in system
- Uses Google email as primary identifier
- Extracts user name, email, and phone (if available) from Google profile
- Returns refresh token valid for 7 days

**Notes:**
- This endpoint requires Google OAuth credentials configured on the backend
- The redirect URI must be whitelisted in Google Console
- Users are created with auto-generated usernames based on their email
- Email verification is skipped for Google OAuth users

---

## Password Reset

### 1. Forgot Password

**Endpoint:** `POST /Auth/forgot-password`

**Description:** Initiates password reset flow by sending an OTP to the registered email.

**Request Body:**
```json
{
  "email": "john@example.com"
}
```

**Response (Success - 200 OK):**
```json
{
  "message": "Password reset OTP sent to your email"
}
```

**Response (Error - 404):**
```json
{
  "message": "User with this email not found"
}
```

---

### 2. Verify Reset OTP

**Endpoint:** `POST /Auth/verify-reset-otp`

**Description:** Verifies the OTP received for password reset.

**Request Body:**
```json
{
  "email": "john@example.com",
  "otp": "123456"
}
```

**Response (Success - 200 OK):**
```json
{
  "resetToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**Response (Error - 401):**
```json
{
  "message": "Invalid or expired OTP"
}
```

**Frontend Flow:**
- User enters email and clicks "Forgot Password"
- Call `/forgot-password` endpoint
- User receives OTP in email
- User enters OTP
- Call `/verify-reset-otp` endpoint
- Store `resetToken` for password reset

---

### 3. Reset Password

**Endpoint:** `POST /Auth/reset-password`

**Description:** Resets the user's password using the reset token from OTP verification.

**Request Body:**
```json
{
  "email": "john@example.com",
  "resetToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "newPassword": "NewSecurePassword123!"
}
```

**Response (Success - 200 OK):**
```json
{
  "message": "Password reset successfully"
}
```

**Response (Error - 401):**
```json
{
  "message": "Invalid reset token"
}
```

**Response (Error - 400):**
```json
{
  "message": "Password does not meet requirements"
}
```

---

## Token Refresh

### Refresh Access Token

**Endpoint:** `POST /Auth/refresh`

**Description:** Generates a new access token using a valid refresh token.

**Request:**
```
POST /Auth/refresh?oldRefreshToken=refresh_token_value
```

Or with request body:
```json
{
  "oldRefreshToken": "refresh_token_value"
}
```

**Response (Success - 200 OK):**
```json
{
  "accessToken": "new_access_token",
  "refreshToken": "new_refresh_token"
}
```

**Response (Error - 401):**
```json
{
  "message": "Invalid or expired refresh token"
}
```

**Frontend Implementation:**
```javascript
async function refreshToken() {
  const oldRefreshToken = localStorage.getItem('refreshToken');
  
  const response = await fetch(`https://api.domain.com/api/Auth/refresh?oldRefreshToken=${oldRefreshToken}`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    }
  });
  
  const data = await response.json();
  if (response.ok) {
    localStorage.setItem('accessToken', data.accessToken);
    localStorage.setItem('refreshToken', data.refreshToken);
  }
  return data;
}
```

---

## Authentication Headers

For all authenticated endpoints (if you add them), include the access token in the Authorization header:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Example:**
```javascript
fetch('https://api.domain.com/api/protected-endpoint', {
  method: 'GET',
  headers: {
    'Authorization': `Bearer ${accessToken}`,
    'Content-Type': 'application/json'
  }
});
```

---

## User Profile

### 1. Get Current User Profile

**Endpoint:** `GET /Users/profile`

**Description:** Retrieves the current logged-in user's profile information.

**Headers Required:**
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json
```

**Response (Success - 200 OK):**
```json
{
  "id": "user-uuid",
  "userName": "john_doe",
  "email": "john@example.com",
  "phoneNumber": "+20123456789",
  "fullName": "John Doe",
  "role": "user",
  "createdAt": "2025-01-15T10:30:00Z",
  "updatedAt": "2025-01-15T10:30:00Z"
}
```

**Response (Error - 401):**
```json
{
  "message": "Unauthorized - Invalid or expired token"
}
```

**Response (Error - 404):**
```json
{
  "message": "User not found"
}
```

**Frontend Implementation:**
```javascript
async function getCurrentUserProfile(accessToken) {
  const response = await fetch('https://api.domain.com/api/Users/profile', {
    method: 'GET',
    headers: {
      'Authorization': `Bearer ${accessToken}`,
      'Content-Type': 'application/json'
    }
  });

  const data = await response.json();
  if (response.ok) {
    return data;
  } else {
    console.error('Error:', data.message);
  }
  return null;
}
```

---

### 2. Update Current User Profile

**Endpoint:** `PUT /Users/profile`

**Description:** Updates the current logged-in user's profile information.

**Headers Required:**
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json
```

**Request Body:**
```json
{
  "fullName": "John Updated",
  "phoneNumber": "+20987654321",
  "userName": "john_updated"
}
```

**Response (Success - 200 OK):**
```json
{
  "id": "user-uuid",
  "userName": "john_updated",
  "email": "john@example.com",
  "phoneNumber": "+20987654321",
  "fullName": "John Updated",
  "role": "user",
  "createdAt": "2025-01-15T10:30:00Z",
  "updatedAt": "2025-01-15T12:45:00Z"
}
```

**Response (Error - 400):**
```json
{
  "message": "Invalid input data"
}
```

**Response (Error - 401):**
```json
{
  "message": "Unauthorized - Invalid or expired token"
}
```

**Response (Error - 404):**
```json
{
  "message": "User not found"
}
```

**Response (Error - 409):**
```json
{
  "message": "Username is already taken"
}
```

**Frontend Implementation:**
```javascript
async function updateUserProfile(accessToken, updateData) {
  const response = await fetch('https://api.domain.com/api/Users/profile', {
    method: 'PUT',
    headers: {
      'Authorization': `Bearer ${accessToken}`,
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(updateData)
  });

  const data = await response.json();
  if (response.ok) {
    return data;
  } else {
    console.error('Error:', data.message);
  }
  return null;
}

// Usage:
const updatedProfile = await updateUserProfile(accessToken, {
  fullName: "John Updated",
  phoneNumber: "+20987654321",
  userName: "john_updated"
});
```

**Notes:**
- Only authenticated users can access this endpoint
- Email cannot be changed through this endpoint
- Username must be unique across the system
- Phone number must include country code
- All fields are optional - only include fields you want to update

---

## Error Handling

### Common HTTP Status Codes

| Status Code | Meaning |
|-------------|---------|
| 200 | Success |
| 400 | Bad Request - Invalid input data |
| 401 | Unauthorized - Invalid credentials or expired token |
| 404 | Not Found - Resource does not exist |
| 500 | Internal Server Error - Server error |

### Error Response Format

```json
{
  "message": "Descriptive error message"
}
```

### Frontend Error Handling Example

```javascript
async function handleApiCall(url, options) {
  try {
    const response = await fetch(url, options);
    const data = await response.json();
    
    if (!response.ok) {
      if (response.status === 401) {
        // Token expired - try to refresh
        const refreshed = await refreshToken();
        if (refreshed.accessToken) {
          // Retry original request with new token
          return handleApiCall(url, options);
        } else {
          // Redirect to login
          window.location.href = '/login';
        }
      } else if (response.status === 400) {
        console.error('Validation error:', data.message);
      } else {
        console.error('Error:', data.message);
      }
    }
    return data;
  } catch (error) {
    console.error('Network error:', error);
    throw error;
  }
}
```

---

## Complete Registration Flow Example

### SMS OTP Registration (New User)
```
1. User enters phone number
   → POST /Auth/send-sms-otp
   
2. User receives OTP via SMS
   → User enters OTP
   
3. Verify OTP
   → POST /Auth/verify-sms-otp
   → Response: is_new_user = true with tempToken
   
4. Redirect to complete profile page
   → Use tempToken in subsequent requests if needed
```

### Email Registration (New User)
```
1. User enters full name, phone, email, password
   → POST /Auth/register
   
2. User receives OTP via email
   → User enters OTP
   
3. Verify Email OTP
   → POST /Auth/VerifyEmailOtp
   → Account is created
   → Response: temp_token for future authenticated requests
   
4. User can now login
   → POST /Auth/login
```

### Password Reset Flow
```
1. User clicks "Forgot Password"
   → User enters email
   → POST /Auth/forgot-password
   
2. User receives reset OTP via email
   → User enters OTP
   
3. Verify Reset OTP
   → POST /Auth/verify-reset-otp
   → Response: resetToken
   
4. Enter New Password
   → POST /Auth/reset-password with resetToken
   
5. Success - User can login with new password
```

---

## Important Notes

- **Token Expiration:** Access tokens typically expire after a set period (e.g., 24 hours). Use the refresh endpoint to get a new token.
- **Refresh Token Storage:** Store refresh tokens securely (preferably in httpOnly cookies).
- **OTP Validity:** All OTPs are valid for 2 minutes. Display countdown to user.
- **Rate Limiting:** Consider implementing rate limiting on your frontend to prevent abuse.
- **Input Validation:** Always validate input on the frontend before sending to API.
- **HTTPS Only:** All endpoints must be accessed over HTTPS in production.
- **CORS:** Ensure your frontend domain is whitelisted in the API's CORS policy.

---

## Testing Endpoints

Use tools like Postman or cURL to test endpoints:

```bash
# Send SMS OTP
curl -X POST https://your-api.com/api/Auth/send-sms-otp \
  -H "Content-Type: application/json" \
  -d '{"phoneNumper":"+20123456789"}'

# Verify SMS OTP
curl -X POST https://your-api.com/api/Auth/verify-sms-otp \
  -H "Content-Type: application/json" \
  -d '{"phoneNumper":"+20123456789","otp":"123456"}'

# Login
curl -X POST https://your-api.com/api/Auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"john@example.com","password":"SecurePassword123!"}'
```

---

## Support

For API issues or questions, contact the backend team or refer to the backend API documentation.

**Last Updated:** 2025
