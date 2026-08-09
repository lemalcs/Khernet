/* Definition for the VERIFY_USER_EXISTENCE procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Verify if a peer is already registered on database.
--
-- Parameters:
-- TOKEN - The token of user.
--
-- Returns:
-- True if user is registered otherwise false.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE VERIFY_USER_EXISTENCE
(
  TOKEN TYPE OF COLUMN PEER.TOKEN
)
RETURNS
(
  USER_EXISTS BOOLEAN
)
AS
DECLARE VARIABLE ID_TOKEN INT;
BEGIN
    FOR SELECT 'TRUE'
   FROM PEER
   WHERE
   TOKEN=:TOKEN
   INTO :USER_EXISTS
   DO
   SUSPEND;
END