/* Definition for the SAVE_PROFILE procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Update the profile of current logged user.
--
-- Parameters:
-- USERNAME - The user name.
-- STATE - The current state (online, offline)
-- SLOGAN - The slogan.
-- GROUPNAME - The group name that user belongs to.
-- FULL_NAME - The full name of user.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE SAVE_PROFILE
(
  USERNAME TYPE OF COLUMN ACCOUNT.USER_NAME,
  STATE TYPE OF COLUMN ACCOUNT.STATE,
  SLOGAN TYPE OF COLUMN ACCOUNT.SLOGAN default null,
  GROUPNAME TYPE OF COLUMN ACCOUNT.GROUP_NAME default null,
  FULL_NAME TYPE OF COLUMN ACCOUNT.FULL_NAME default null
)
AS
BEGIN
   UPDATE ACCOUNT
   SET USER_NAME=:USERNAME,
   SLOGAN=:SLOGAN,
   GROUP_NAME=:GROUPNAME,
   FULL_NAME=:FULL_NAME,
   STATE=:STATE;
END