/* Definition for the GET_ACCOUNT_PROFILE procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Get the profile information of logged user.
--
-- Returns:
-- Profile fields
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE GET_ACCOUNT_PROFILE
RETURNS
(
  TOKEN TYPE OF COLUMN ACCOUNT.TOKEN,
  USER_NAME TYPE OF COLUMN ACCOUNT.USER_NAME,
  STATE TYPE OF COLUMN ACCOUNT.STATE,
  SLOGAN TYPE OF COLUMN ACCOUNT.SLOGAN,
  GROUP_NAME TYPE OF COLUMN ACCOUNT.GROUP_NAME,
  FULL_NAME TYPE OF COLUMN ACCOUNT.FULL_NAME
)
AS
BEGIN
   FOR SELECT
   TOKEN,
   USER_NAME,
   STATE,
   SLOGAN,
   GROUP_NAME,
   FULL_NAME
   FROM ACCOUNT
   INTO
   :TOKEN,
   :USER_NAME,
   :STATE,
   :SLOGAN,
   :GROUP_NAME,
   :FULL_NAME
   DO
   SUSPEND;
END