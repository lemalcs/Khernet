/* Definition for the SAVE_PEER_PROFILE procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Update the profile of peer.
--
-- Parameters:
-- TOKEN - The token of peer.
-- USERNAME - The user name of peer.
-- STATE - The current state of peer (online, offline)
-- SLOGAN - The slogan published by peer.
-- GROUPNAME - The group name that peer belongs to.
-- FULL_NAME - The full name of peer.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE SAVE_PEER_PROFILE
(
  TOKEN TYPE OF COLUMN PEER.TOKEN,
  USERNAME TYPE OF COLUMN PEER.USER_NAME,
  STATE TYPE OF COLUMN PEER.STATE,
  SLOGAN TYPE OF COLUMN PEER.SLOGAN default null,
  GROUPNAME TYPE OF COLUMN PEER.GROUP_NAME default null,
  FULL_NAME TYPE OF COLUMN PEER.FULL_NAME default null
)
AS
BEGIN
   UPDATE PEER
   SET USER_NAME=:USERNAME,
   SLOGAN=:SLOGAN,
   GROUP_NAME=:GROUPNAME,
   STATE=:STATE,
   FULL_NAME=:FULL_NAME
   WHERE
   TOKEN=:TOKEN;
END