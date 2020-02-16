/* Definition for the GET_UID_MESSAGE procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Get the UID (Universal ID) of message.
--
-- Parameters:
-- ID_MESSAGE - The internal id of message.
--
-- Returns:
-- The UID of message
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE GET_UID_MESSAGE
(
  ID_MESSAGE TYPE OF COLUMN MESSAGE.ID
)
RETURNS
(
  UID TYPE OF COLUMN MESSAGE.UID
)
AS
BEGIN
   FOR SELECT UID FROM MESSAGE 
   WHERE
   ID=:ID_MESSAGE
   INTO :UID
   DO 
   SUSPEND;
END