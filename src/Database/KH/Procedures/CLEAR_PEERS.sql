/* Definition for the CLEAR_PEERS procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Deletes peer information like address, name and so on.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE CLEAR_PEERS
AS
BEGIN
   DELETE FROM PEER_ADDRESS;
   DELETE FROM PEER;
END