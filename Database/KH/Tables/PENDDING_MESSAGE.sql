/* Structure for the PENDDING_MESSAGE table :  */

CREATE TABLE PENDDING_MESSAGE 
(
  ID_RECEIPT INTEGER NOT NULL,
  ID_MESSAGE INTEGER NOT NULL,
  REG_DATE TIMESTAMP NOT NULL
);


ALTER TABLE PENDDING_MESSAGE ADD CONSTRAINT PK_PENDDING_MESSAGE PRIMARY KEY (ID_RECEIPT,ID_MESSAGE);