INSERT INTO Department ([Name], Budget)
	VALUES ('Gardening', 5670000),
			('Technology', 3945830),
			('Animal Control', 1928128),
			('Spirituality', 10000);


INSERT INTO Employee (FirstName, LastName, DepartmentId, IsSuperVisor)
	VALUES ('Allison', 'Collins', 4, 'true'),
			('Jordan', 'Rosas', 3, 'true'),
			('Brittany', 'Ramos-Janeway', 2,'true'),
			('Nick', 'Hanson', 1, 'true'),
			('Asia', 'Carter', 4, 'false'),
			('Hunter', 'Daddy', 3, 'false'),
			('Megan', 'Cruzen', 2, 'false'),
			('Justin Dance', 'Wheeler', 1, 'false');

INSERT INTO Computer (PurchaseDate, DecomissionDate, Make, Manufacturer)
	VALUES ('2015-10-20 00:00:00', null, 'Inspiron', 'Dell'),
			('2015-10-20 00:00:00', null, 'Surface', 'Microsoft'),
			('2015-10-20 00:00:00', null, 'XPS', 'Dell'),
			('2015-10-20 00:00:00', null, 'PixelBook', 'Google'),
			('2015-10-20 00:00:00', null, 'YOga', 'HP'),
			('2015-10-20 00:00:00', null, 'Toughbook', 'Lenovo'),
			('2015-10-20 00:00:00', null, 'Viao', 'Sony'),
			('2015-10-20 00:00:00', null, 'Alienware', 'Dell'),
			('2018-01-30 00:00:00', null, 'Macbook Pro', 'Apple'),
			('2018-01-30 00:00:00', null, 'Macbook Air', 'Apple'),
			('2018-01-30 00:00:00', null, 'Macbook', 'Apple'),
			('2018-01-30 00:00:00', null, 'Macbook Mini', 'Apple');

INSERT INTO ComputerEmployee (EmployeeId, ComputerId, AssignDate, UnassignDate)
	VALUES (1, 1, '2018-01-30 00:00:00', null),
			(2, 2, '2018-01-30 00:00:00', null),
			(3, 3, '2018-01-30 00:00:00', null),
			(4, 4, '2018-01-30 00:00:00', null),
			(5, 5, '2010-01-30 00:00:00', null),
			(6, 6, '2010-01-30 00:00:00', null),
			(7, 7, '2010-01-30 00:00:00', null),
			(8, 8, '2010-01-30 00:00:00', null);

INSERT INTO TrainingProgram ([Name], StartDate, EndDate, MaxAttendees)
	VALUES ('How to Train Your Dragon', '2019-04-08 00:00:00', '2019-04-13 00:00:00', 3),
			('Avoid Being Catfished', '2019-05-01 00:00:00', '2019-06-01 00:00:00', 6),
			('Learn How to Text', '2019-06-03 00:00:00', '2019-06-04 00:00:00', 2);

INSERT INTO EmployeeTraining (EmployeeId, TrainingProgramId)
	VALUES (1, 1),
			(3, 1),
			(5, 1),
			(2, 2),
			(6, 2),
			(8, 3),
			(6, 3);

INSERT INTO Customer (FirstName, LastName) VALUES ('Aaron', 'Carter');
INSERT INTO Customer (FirstName, LastName) VALUES ('Britney', 'Spears');
INSERT INTO Customer (FirstName, LastName) VALUES ('Justin', 'Timberlake');
INSERT INTO Customer (FirstName, LastName) VALUES ('Jennifer', 'Lopez');
INSERT INTO Customer (FirstName, LastName) VALUES ('Paula', 'Abdul');

INSERT INTO ProductType ([Name]) VALUES ('Electronics');
INSERT INTO ProductType ([Name]) VALUES ('Home Goods');
INSERT INTO ProductType ([Name]) VALUES ('Clothing');

INSERT INTO Product (ProductTypeId, CustomerId, Price, Title, [Description], Quantity) VALUES (1, 1, 50, 'Spy Glasses', 'See what your neighbors are up to', 4);
INSERT INTO Product (ProductTypeId, CustomerId, Price, Title, [Description], Quantity) VALUES (1, 2, 100, 'Alienware 7X 2.0', 'Tap into the ISS feed from your PC', 1);
INSERT INTO Product (ProductTypeId, CustomerId, Price, Title, [Description], Quantity) VALUES (2, 3, 20, 'Chia Pet', 'Grow your best friend', 500);
INSERT INTO Product (ProductTypeId, CustomerId, Price, Title, [Description], Quantity) VALUES (2, 4, 30, 'Ant Farm', 'Become the ruler of a tiny insect world', 2);
INSERT INTO Product (ProductTypeId, CustomerId, Price, Title, [Description], Quantity) VALUES (3, 5, 150, 'Apple Bottom Jeans', 'Pair with fur boots', 3);
INSERT INTO Product (ProductTypeId, CustomerId, Price, Title, [Description], Quantity) VALUES (3, 5, 200, 'Boots with the Fur', 'Pair with apple bottom jeans', 20);

INSERT INTO PaymentType (AcctNumber, [Name], CustomerId) VALUES (8321, 'American Express', 2);
INSERT INTO PaymentType (AcctNumber, [Name], CustomerId) VALUES (4362, 'Visa', 1);
INSERT INTO PaymentType (AcctNumber, [Name], CustomerId) VALUES (1362, 'Mastercard', 2);
INSERT INTO PaymentType (AcctNumber, [Name], CustomerId) VALUES (1038, 'Visa', 3);
INSERT INTO PaymentType (AcctNumber, [Name], CustomerId) VALUES (2741, 'PayPal', 4);
INSERT INTO PaymentType (AcctNumber, [Name], CustomerId) VALUES (1719, 'Visa', 5);

INSERT INTO [Order] (CustomerId, PaymentTypeId) VALUES (1, NULL);
INSERT INTO [Order] (CustomerId, PaymentTypeId) VALUES (2, 3);
INSERT INTO [Order] (CustomerId, PaymentTypeId) VALUES (3, 4);
INSERT INTO [Order] (CustomerId, PaymentTypeId) VALUES (4, 5);
INSERT INTO [Order] (CustomerId, PaymentTypeId) VALUES (5, 6);

INSERT INTO OrderProduct (OrderId, ProductId) VALUES (1, 1);
INSERT INTO OrderProduct (OrderId, ProductId) VALUES (2, 2);
INSERT INTO OrderProduct (OrderId, ProductId) VALUES (3, 3);
INSERT INTO OrderProduct (OrderId, ProductId) VALUES (4, 4);
INSERT INTO OrderProduct (OrderId, ProductId) VALUES (5, 5);
INSERT INTO OrderProduct (OrderId, ProductId) VALUES (5, 6);

SELECT * FROM Employee;
SELECT * FROM Department;
SELECT * FROM Computer;
SELECT * FROM TrainingProgram;
SELECT * FROM ComputerEmployee;
SELECT * FROM EmployeeTraining;
SELECT * FROM Product;
SELECT * FROM PaymentType;
SELECT * FROM [Order];
SELECT * FROM ProductType;
