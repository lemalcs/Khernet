/* Definition for the SET_MESSAGE_STATE procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Update the state of a message.
--
-- Parameters:
-- ID_MESSAGE - The id of message.
-- STATE - The state of message (processed, pendding, error).
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE SET_MESSAGE_STATE
(
  ID_MESSAGE INTEGER,
  STATE INTEGER
)
AS
BEGIN
   UPDATE MESSAGE
   SET STATE=:STATE
   WHERE
   ID=:ID_MESSAGE;
END