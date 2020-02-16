/* Definition for the GET_ACCOUNT_AVATAR procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Get the avatar of logged user.
--
-- Returns:
-- A byte array of avatar (image file).
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE GET_ACCOUNT_AVATAR
RETURNS
(
  AVATAR TYPE OF COLUMN ACCOUNT.AVATAR
)
AS
BEGIN
   FOR SELECT AVATAR FROM ACCOUNT
   INTO :AVATAR
   DO
   SUSPEND;
END