/* Definition for the SET_ACCOUNT_STATE procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Update the state of current logged user.
--
-- Parameters:
-- STATE - The state of user (online, offline).
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE SET_ACCOUNT_STATE
(
  STATE TYPE OF COLUMN PEER.STATE
)
AS
BEGIN
   UPDATE ACCOUNT
   SET STATE=:STATE;
END