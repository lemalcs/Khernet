/* Definition for the SAVE_AVATAR procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Save the avtar of current logged user.
--
-- Parameters:
-- AVATAR - The avatar (image file).
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE SAVE_AVATAR
(
  AVATAR TYPE OF COLUMN PEER.AVATAR
)
AS
BEGIN
   UPDATE ACCOUNT
   SET AVATAR=:AVATAR;
END