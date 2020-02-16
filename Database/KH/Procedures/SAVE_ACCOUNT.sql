/* Definition for the SAVE_ACCOUNT procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Save the account information of current logged user.
--
-- Parameters:
-- USER_NAME - The user name.
-- TOKEN - The token which is a combination of numbers a letters
-- CERTIFICATE - The X509 certificate of the user.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE SAVE_ACCOUNT
(
  USER_NAME TYPE OF COLUMN ACCOUNT.USER_NAME,
  TOKEN TYPE OF COLUMN ACCOUNT.TOKEN,
  CERTIFICATE TYPE OF COLUMN CREDENTIAL.CERT
)
AS

DECLARE VARIABLE TODAY TIMESTAMP;
DECLARE VARIABLE ID_TOKEN INT;

BEGIN
   TODAY=(SELECT CURRENT_TIMESTAMP FROM RDB$DATABASE);

   INSERT INTO ACCOUNT(TOKEN,USER_NAME,REG_DATE)
   VALUES(:TOKEN,:USER_NAME,:TODAY)
   RETURNING ID
   INTO :ID_TOKEN;
   
   INSERT INTO CREDENTIAL(ID_ACCOUNT,CERT)
   VALUES(:ID_TOKEN,:CERTIFICATE);
END