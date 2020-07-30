/* Definition for the GET_TIMEID_MESSAGE procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-07-26
-- Autor: Luis Lema
--
-- Description: 
-- Get the number o ticks (100 nanoseconds) of the message.
--
-- Parameters:
-- ID_MESSAGE - The id of message.
--
-- Returns:
-- The number of ticks based on UTCS.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE GET_TIMEID_MESSAGE
(
  ID_MESSAGE TYPE OF COLUMN MESSAGE.ID  
)
RETURNS
(
  TIMEID_MESSAGE TYPE OF COLUMN MESSAGE.TIMEID
)
AS
BEGIN
   FOR SELECT TIMEID FROM MESSAGE 
   WHERE
   ID=:ID_MESSAGE
   INTO :TIMEID_MESSAGE
   DO 
   SUSPEND;
END