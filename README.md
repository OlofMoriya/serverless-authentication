# ServerlessAuthentication

## GenerateToken 
- generates a token from basic auth

## AddUser 
- Creates a user in the system. 

Link to postman collection for api calls
https://www.getpostman.com/collections/eaa678e1962026d66e4e


Todo: 

- [ ] General cleanup.
- [ ] Add some authentication and limitation to AddUser.
- [ ] Use a key chain for token secret.
- [ ] Add configuration for token-specifics such as issuer and audience.
- [ ] Handle user creation with correct status.
- [ ] Add a pepper to the password hash.
- [ ] Implement ValidateData in UserData.
