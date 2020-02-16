/* Definition for the BI_CONNSTR_ID trigger :  */

SET TERM ^ ;

CREATE TRIGGER BI_CONNSTR_ID FOR CONNSTR
ACTIVE BEFORE 
  INSERT
POSITION 0
AS
BEGIN
  IF (NEW.ID IS NULL) THEN
      NEW.ID = GEN_ID(CONNSTR_ID_GEN, 1);
END