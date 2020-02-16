/* Definition for the GET_PEER_PROFILE procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Get the profile of a peer.
--
-- Parameters:
-- TOKEN - The token of peer.
--
-- Returns:
-- The profile details of peer.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE GET_PEER_PROFILE
(
  TOKEN TYPE OF COLUMN PEER.TOKEN
)
RETURNS
(
  USER_NAME TYPE OF COLUMN PEER.USER_NAME,
  STATE TYPE OF COLUMN PEER.STATE,
  SLOGAN TYPE OF COLUMN PEER.SLOGAN,
  GROUP_NAME TYPE OF COLUMN PEER.GROUP_NAME,
  FULL_NAME TYPE OF COLUMN PEER.FULL_NAME,
  DISPLAY_NAME TYPE OF COLUMN PEER.DISPLAY_NAME,
  INITIALS TYPE OF COLUMN PEER.INITIALS,
  HEX_COLOR TYPE OF COLUMN PEER.HEX_COLOR
)
AS
begin
   for SELECT
   user_name,
   state,
   slogan,
   group_name,
   full_name,
   display_name,
   initials,
   hex_color
   from peer
   where
   token=:token
   INTO
   :user_name,
   :state,
   :slogan,
   :group_name,
   :full_name,
   :display_name,
   :INITIALS,
   :hex_color
   do
   suspend;
end