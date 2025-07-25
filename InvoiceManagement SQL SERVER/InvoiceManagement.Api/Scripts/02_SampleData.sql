-- =====================================================
-- Script de Datos de Ejemplo - Invoice Management System
-- Inserción de datos basados en bd_exam_invoices.json
-- =====================================================

USE InvoiceManagementDB;
GO

-- Limpiar datos existentes (opcional)
DELETE FROM InvoicePayments;
DELETE FROM CreditNotes;
DELETE FROM InvoiceDetails;
DELETE FROM Invoices;
DELETE FROM Customers;

-- Reiniciar contadores de identidad
DBCC CHECKIDENT ('InvoicePayments', RESEED, 0);
DBCC CHECKIDENT ('CreditNotes', RESEED, 0);
DBCC CHECKIDENT ('InvoiceDetails', RESEED, 0);
DBCC CHECKIDENT ('Invoices', RESEED, 0);
DBCC CHECKIDENT ('Customers', RESEED, 0);

-- =====================================================
-- INSERCIÓN DE CLIENTES
-- =====================================================

INSERT INTO Customers (CustomerRun, CustomerName, CustomerEmail) VALUES
('13075795-2', 'Juanita Hugh', 'jhugh0@xinhuanet.com'),
('15012308-9', 'Dacey Bygraves', 'dbygraves1@ca.gov'),
('16945527-9', 'Patti Boyson', 'pboyson2@wsj.com'),
('9G13WC4RU71', 'Zsa zsa Keme', 'zzsa3@sun.com'),
('13785469-4', 'Dynah Lack', 'dlack4@wufoo.com'),
('75204688-3', 'Perry Curado', 'pcurado1d@forbes.com'),
('84860403-8', 'Stinky McCorrie', 'smccorrie15@telegraph.co.uk'),
('87799877-0', 'Lon Davidai', 'ldavidai18@mlb.com');

-- =====================================================
-- INSERCIÓN DE FACTURAS
-- =====================================================

INSERT INTO Invoices (InvoiceNumber, InvoiceDate, InvoiceStatus, TotalAmount, DaysToDue, PaymentDueDate, PaymentStatus, IsConsistent, CustomerId) VALUES
(1, '2025-01-14', 'issued', 2650.19, 75, '2025-07-31', 'Pending', 1, 1),
(2, '2025-03-05', 'partial', 2250.45, 14, '2025-03-19', 'Paid', 1, 2),
(3, '2024-12-15', 'issued', 4333.64, 55, '2025-06-30', 'Pending', 1, 3),
(4, '2025-02-21', 'issued', 3286.56, 17, '2025-03-10', 'Paid', 1, 4),
(5, '2025-02-10', 'issued', 129.36, 66, '2025-04-17', 'Paid', 1, 5),
(50, '2025-02-18', 'issued', 4212.54, 26, '2025-03-16', 'Pending', 1, 6),
(42, '2024-12-27', 'issued', 608.42, 45, '2025-02-10', 'Pending', 1, 7),
(45, '2024-12-05', 'issued', 6252.28, 22, '2024-12-27', 'Pending', 1, 8);

-- =====================================================
-- INSERCIÓN DE DETALLES DE FACTURA
-- =====================================================

INSERT INTO InvoiceDetails (ProductName, UnitPrice, Quantity, Subtotal, InvoiceId) VALUES
-- Factura 1
('Lettuce - Baby Salad Greens', 31.93, 83, 2650.19, 1),
-- Factura 2
('Appetizer - Shrimp Puff', 150.03, 15, 2250.45, 2),
-- Factura 3
('Cake - Mini Cheesecake', 127.46, 34, 4333.64, 3),
-- Factura 4
('Fruit Mix - Light', 68.47, 48, 3286.56, 4),
-- Factura 5
('Bread - Raisin Walnut Pull', 4.62, 28, 129.36, 5),
-- Factura 50
('Soup - Campbells - Chicken Noodle', 72.63, 58, 4212.54, 6),
-- Factura 42
('Wine - Gewurztraminer Pierre', 10.49, 58, 608.42, 7),
-- Factura 45
('Wine - Jaboulet Cotes Du Rhone', 60.38, 81, 4890.78, 8),
('Wine - Winzer Krems Gruner', 27.23, 50, 1361.50, 8);

-- =====================================================
-- INSERCIÓN DE PAGOS
-- =====================================================

INSERT INTO InvoicePayments (PaymentMethod, PaymentDate, InvoiceId) VALUES
(NULL, NULL, 1),  -- Factura 1: Sin pago
('Apple Pay', '2025-03-05', 2),  -- Factura 2: Pagada
(NULL, NULL, 3),  -- Factura 3: Sin pago
('Google Pay', '2025-02-21', 4),  -- Factura 4: Pagada
('Credit Card', '2025-02-10', 5),  -- Factura 5: Pagada
(NULL, NULL, 6),  -- Factura 50: Sin pago
(NULL, NULL, 7),  -- Factura 42: Sin pago
(NULL, NULL, 8);  -- Factura 45: Sin pago

-- =====================================================
-- INSERCIÓN DE NOTAS DE CRÉDITO
-- =====================================================

INSERT INTO CreditNotes (CreditNoteNumber, CreditNoteDate, CreditNoteAmount, InvoiceId) VALUES
(5941, '2025-03-18', 1931.78, 2),  -- NC para factura 2
(2297, '2025-01-10', 1276.19, 6);  -- NC para factura 50

-- =====================================================
-- CONSULTAS DE VERIFICACIÓN
-- =====================================================

-- Resumen de datos insertados
SELECT 'Datos de ejemplo insertados exitosamente' AS Status;

SELECT 
    'Customers' AS TablaName, 
    COUNT(*) AS RegistrosInsertados 
FROM Customers
UNION ALL
SELECT 
    'Invoices' AS TablaName, 
    COUNT(*) AS RegistrosInsertados 
FROM Invoices
UNION ALL
SELECT 
    'InvoiceDetails' AS TablaName, 
    COUNT(*) AS RegistrosInsertados 
FROM InvoiceDetails
UNION ALL
SELECT 
    'InvoicePayments' AS TablaName, 
    COUNT(*) AS RegistrosInsertados 
FROM InvoicePayments
UNION ALL
SELECT 
    'CreditNotes' AS TablaName, 
    COUNT(*) AS RegistrosInsertados 
FROM CreditNotes;

-- Vista resumida de facturas con clientes
SELECT 
    i.InvoiceNumber,
    c.CustomerName,
    i.TotalAmount,
    i.PaymentStatus,
    i.InvoiceStatus
FROM Invoices i
INNER JOIN Customers c ON i.CustomerId = c.Id
ORDER BY i.InvoiceNumber;

GO