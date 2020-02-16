/* Definition for the CLEAR_PEER_STATE procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Set all peers' state as offline
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE CLEAR_PEER_STATE
AS
BEGIN
   UPDATE PEER
   SET STATE=0; --Offline state
END