/* Definition for the GET_ANIMATION_LIST procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Get the list of animations (AVI files).
--
-- Returns:
-- A list of ids (numbers).
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE GET_ANIMATION_LIST
RETURNS
(
  ID INTEGER
)
AS
BEGIN
   FOR 
   SELECT ID
   FROM ANIMATION
   INTO :ID
   DO
   SUSPEND;
END