/* Structure for the PENDING_MESSAGE table :  */

CREATE TABLE PENDING_MESSAGE 
(
  ID_RECEIPT INTEGER NOT NULL,
  ID_MESSAGE INTEGER NOT NULL,
  REG_DATE TIMESTAMP NOT NULL
);


ALTER TABLE PENDING_MESSAGE ADD CONSTRAINT PK_PENDING_MESSAGE PRIMARY KEY (ID_RECEIPT,ID_MESSAGE);