/* Definition for the SAVE_PEER_AVATAR procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Save the avatar (image) of peer.
--
-- Parameters:
-- TOKEN - The token of peer.
-- AVATAR - The image of peer profile.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE SAVE_PEER_AVATAR
(
  TOKEN TYPE OF COLUMN PEER.TOKEN,
  AVATAR TYPE OF COLUMN PEER.AVATAR
)
AS
BEGIN
   UPDATE PEER
   SET AVATAR=:AVATAR
   WHERE
   TOKEN=:TOKEN;
END