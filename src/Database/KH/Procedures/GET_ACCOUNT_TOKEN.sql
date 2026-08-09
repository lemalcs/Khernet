/* Definition for the GET_ACCOUNT_TOKEN procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Get the token of logged user.
--
-- Returns:
-- User name, token and certificate.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE GET_ACCOUNT_TOKEN
RETURNS
(
  USER_NAME TYPE OF COLUMN ACCOUNT.USER_NAME,
  TOKEN TYPE OF COLUMN ACCOUNT.TOKEN,
  CERT TYPE OF COLUMN CREDENTIAL.CERT
)
AS
BEGIN
   FOR SELECT 
   A.USER_NAME,
   A.TOKEN,
   B.CERT
   FROM ACCOUNT A JOIN CREDENTIAL B
   ON A.ID=B.ID_ACCOUNT
   INTO
   :USER_NAME, :TOKEN,:CERT
   DO
   SUSPEND;
END