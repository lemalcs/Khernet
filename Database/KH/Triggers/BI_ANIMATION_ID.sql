/* Definition for the BI_ANIMATION_ID trigger :  */

CREATE TRIGGER BI_ANIMATION_ID FOR ANIMATION
ACTIVE BEFORE 
  INSERT
POSITION 0
AS
BEGIN
  IF (NEW.ID IS NULL) THEN
      NEW.ID = GEN_ID(ANIMATION_ID_GEN, 1);
END