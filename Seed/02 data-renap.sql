-- insert a citizen into citizen table using postgres sintax

INSERT INTO TBL_CITIZEN (cui, first_name, last_name, date_of_birth, date_of_decease, nationality)
VALUES 
('1234567890123', 'John', 'Doe', '1990-01-13', NULL, 'Guatemalteco/a'),
('9876543210987', 'Jane', 'Smith', '1985-05-20', NULL, 'Guatemalteco/a'),
('4567890123456', 'Michael', 'Johnson', '1995-09-02', NULL, 'Guatemalteco/a'),
('7890123456789', 'Emily', 'Brown', '1988-12-10', NULL, 'Guatemalteco/a'),
('6543210987654', 'William', 'Martinez', '1992-03-15', NULL, 'Guatemalteco/a'),
('3210987654321', 'Sophia', 'Garcia', '1998-07-25', NULL, 'Guatemalteco/a'),
('2109876543210', 'James', 'Rodriguez', '1991-11-30', NULL, 'Guatemalteco/a'),
('5432109876543', 'Olivia', 'Lopez', '1996-04-05', NULL, 'Guatemalteco/a'),
('8765432109876', 'Benjamin', 'Perez', '1987-08-18', NULL, 'Guatemalteco/a'),
('2345678901234', 'Isabella', 'Gonzalez', '1994-06-22', NULL, 'Guatemalteco/a'),
('9876543210912', 'Jane', 'Smith', '1985-05-20', '2004-03-13', 'Guatemalteco/a');