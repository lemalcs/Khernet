/* Definition for the UPDATE_PEER_STATE procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Update the state of peer.
--
-- Parameters:
-- TOKEN - The token of peer.
-- STATE - The new state of peer (online, offline).
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE UPDATE_PEER_STATE
(
  TOKEN TYPE OF COLUMN PEER.TOKEN,
  STATE TYPE OF COLUMN PEER.STATE
)
AS
BEGIN
   UPDATE PEER
   SET STATE=:STATE
   WHERE
   TOKEN=:TOKEN;
END