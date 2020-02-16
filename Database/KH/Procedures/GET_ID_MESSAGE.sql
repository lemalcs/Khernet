/* Definition for the GET_ID_MESSAGE procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Get the internal id of message from the universal id (public id).
--
-- Parameters:
-- UID_MESSAGE - The UID (Universal ID) of message
--
-- Returns:
-- The internal id of message
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE GET_ID_MESSAGE
(
  UID_MESSAGE TYPE OF COLUMN MESSAGE.UID
)
RETURNS
(
  ID_MESSAGE TYPE OF COLUMN MESSAGE.ID
)
AS
BEGIN
   FOR SELECT ID FROM MESSAGE 
   WHERE
   UID=:UID_MESSAGE
   INTO :ID_MESSAGE
   DO 
   SUSPEND;
END