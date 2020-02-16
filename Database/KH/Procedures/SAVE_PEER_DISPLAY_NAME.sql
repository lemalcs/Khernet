/* Definition for the SAVE_PEER_DISPLAY_NAME procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Save the peer name to be displayed only for current logged user.
--
-- Parameters:
-- TOKEN - The token of peer.
-- DISPLAY_NAME - The new name of peer.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE SAVE_PEER_DISPLAY_NAME(
  TOKEN TYPE OF COLUMN PEER.TOKEN,
  DISPLAY_NAME TYPE OF COLUMN PEER.DISPLAY_NAME)
AS
BEGIN
   UPDATE PEER
   SET DISPLAY_NAME=:DISPLAY_NAME
   WHERE
   TOKEN=:TOKEN;
END