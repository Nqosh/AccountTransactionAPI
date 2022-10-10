# AccountTransactionAPI
You can download the solution by doing a git clone on (powershell, gitbash or any other tool) or download the zip file To test the api , run F5 on Visual studio and paste this url https://localhost:44322/swagger/index.html or if else the port is different from this one change the port number an example of this would be (portnumber)/swagger/index.html
# Steps to test API
1) You first have to register yourself using the register endpoint below 
![image](https://user-images.githubusercontent.com/31209722/194860749-85124da9-c42f-45d8-86f0-b890135a23b2.png)
2) Then you have to log in using the Login end point below 
![image](https://user-images.githubusercontent.com/31209722/194860840-1a01f5d0-c6a4-47ce-b12b-fa7b4003dccd.png)
3) Then you have to create an Account using the create Account end point below 
![image](https://user-images.githubusercontent.com/31209722/194860971-75311292-5ef5-49ec-9c90-9c8e929c2544.png)
4 Then you can transact with the deposit or withdrawal end point below 
![image](https://user-images.githubusercontent.com/31209722/194861120-f8319b08-9087-42c9-a553-3e2690f314d6.png)
5) To deposit here is an example of how you would fille the swagger form below
The Userid is generated for you in the response of the Login end point
You can use the below link to generate a guid for the refrenceId 
https://www.guidgenerator.com/online-guid-generator.aspx
# On the Transaction Types 
- 1 is for Withdrawal 
- 2 is for Deposit
6) The below example is a deposit transaction to the account created 
![image](https://user-images.githubusercontent.com/31209722/194861710-d5471e5c-3aae-4e78-b1e3-82154a3b4ea4.png)
7) An example of a Withdrawal transaction looks like the one below 
![image](https://user-images.githubusercontent.com/31209722/194861865-e940b8bb-ec03-4f54-b6cf-ede5edfbacdc.png)
8) You can check the account balance with the check accoutn balance endpoint below
![image](https://user-images.githubusercontent.com/31209722/194861997-dc844b33-79c7-4325-b26e-9da29ae3b55a.png)
9) You can also get the Acoount statement or Transactions with the below end point 
![image](https://user-images.githubusercontent.com/31209722/194862112-310091f7-c51b-480f-839a-3fb48c3a398c.png)


