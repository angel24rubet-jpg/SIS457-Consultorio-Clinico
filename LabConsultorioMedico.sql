
-- =========================================
-- 1. CREAR BASE DE DATOS
-- =========================================
CREATE DATABASE LabConsultorioMedico;
GO

USE LabConsultorioMedico;
GO

-- =========================================
-- 2. LOGIN Y USUARIO SQL
-- =========================================
CREATE LOGIN usrconsultoriomedico
WITH PASSWORD = '123456',
DEFAULT_DATABASE = LabConsultorioMedico,
CHECK_EXPIRATION = OFF,
CHECK_POLICY = ON;
GO

CREATE USER usrconsultoriomedico FOR LOGIN usrconsultoriomedico;
GO

ALTER ROLE db_owner ADD MEMBER usrconsultoriomedico;
GO

-- =========================================
-- 3. TABLAS
-- =========================================

CREATE TABLE Especialidad (
  id INT PRIMARY KEY IDENTITY(1,1),
  nombre VARCHAR(30) NOT NULL
);

CREATE TABLE Concepto (
  id INT PRIMARY KEY IDENTITY(1,1),
  idEspecialidad INT NOT NULL,
  descripcion VARCHAR(250) NOT NULL,
  costo DECIMAL(10,2) NOT NULL,
  FOREIGN KEY (idEspecialidad) REFERENCES Especialidad(id)
);

CREATE TABLE Paciente (
  id INT PRIMARY KEY IDENTITY(1,1),
  cedulaIdentidad VARCHAR(12) NOT NULL,
  nombreCompletoPaciente VARCHAR(30) NOT NULL,
  direccion VARCHAR(250) NOT NULL,
  celular BIGINT NOT NULL
);

CREATE TABLE Doctor (
  id INT PRIMARY KEY IDENTITY(1,1),
  idEspecialidad INT NOT NULL,
  cedulaIdentidad VARCHAR(12) NOT NULL,
  nombreCompletoDoctor VARCHAR(30) NOT NULL,
  direccion VARCHAR(250) NOT NULL,
  celular BIGINT NOT NULL,
  FOREIGN KEY (idEspecialidad) REFERENCES Especialidad(id)
);

CREATE TABLE Usuario (
  id INT PRIMARY KEY IDENTITY(1,1),
  idDoctor INT NOT NULL,
  usuario VARCHAR(20) NOT NULL,
  clave VARCHAR(250) NOT NULL,
  FOREIGN KEY (idDoctor) REFERENCES Doctor(id)
);

CREATE TABLE Cita (
  id INT PRIMARY KEY IDENTITY(1,1),
  idDoctor INT NOT NULL,
  idPaciente INT NOT NULL,
  idEspecialidad INT NOT NULL,
  fecha DATE NOT NULL,
  hora TIME NOT NULL,
  FOREIGN KEY (idDoctor) REFERENCES Doctor(id),
  FOREIGN KEY (idPaciente) REFERENCES Paciente(id),
  FOREIGN KEY (idEspecialidad) REFERENCES Especialidad(id)
);

CREATE TABLE HistorialClinico (
  id INT PRIMARY KEY IDENTITY(1,1),
  descripcion VARCHAR(250),
  diagnostico VARCHAR(250),
  tratamiento VARCHAR(250),
  fecha DATE DEFAULT GETDATE(),
  idPaciente INT NOT NULL,
  FOREIGN KEY (idPaciente) REFERENCES Paciente(id)
);

CREATE TABLE Pago (
  id INT PRIMARY KEY IDENTITY(1,1),
  idCita INT NOT NULL,
  idConcepto INT NOT NULL,
  fecha DATE DEFAULT GETDATE(),
  FOREIGN KEY (idCita) REFERENCES Cita(id),
  FOREIGN KEY (idConcepto) REFERENCES Concepto(id)
);

-- =========================================
-- 4. CAMPOS AUDITORÍA
-- =========================================

ALTER TABLE Especialidad ADD usuarioRegistro VARCHAR(50) NOT NULL DEFAULT SUSER_NAME();
ALTER TABLE Especialidad ADD fechaRegistro DATETIME NOT NULL DEFAULT GETDATE();
ALTER TABLE Especialidad ADD estado SMALLINT NOT NULL DEFAULT 1;

ALTER TABLE Paciente ADD usuarioRegistro VARCHAR(50) NOT NULL DEFAULT SUSER_NAME();
ALTER TABLE Paciente ADD fechaRegistro DATETIME NOT NULL DEFAULT GETDATE();
ALTER TABLE Paciente ADD estado SMALLINT NOT NULL DEFAULT 1;

ALTER TABLE Doctor ADD usuarioRegistro VARCHAR(50) NOT NULL DEFAULT SUSER_NAME();
ALTER TABLE Doctor ADD fechaRegistro DATETIME NOT NULL DEFAULT GETDATE();
ALTER TABLE Doctor ADD estado SMALLINT NOT NULL DEFAULT 1;

ALTER TABLE Usuario ADD usuarioRegistro VARCHAR(50) NOT NULL DEFAULT SUSER_NAME();
ALTER TABLE Usuario ADD fechaRegistro DATETIME NOT NULL DEFAULT GETDATE();
ALTER TABLE Usuario ADD estado SMALLINT NOT NULL DEFAULT 1;

ALTER TABLE Cita ADD usuarioRegistro VARCHAR(50) NOT NULL DEFAULT SUSER_NAME();
ALTER TABLE Cita ADD fechaRegistro DATETIME NOT NULL DEFAULT GETDATE();
ALTER TABLE Cita ADD estado SMALLINT NOT NULL DEFAULT 1;

ALTER TABLE Pago ADD usuarioRegistro VARCHAR(50) NOT NULL DEFAULT SUSER_NAME();
ALTER TABLE Pago ADD fechaRegistro DATETIME NOT NULL DEFAULT GETDATE();
ALTER TABLE Pago ADD estado SMALLINT NOT NULL DEFAULT 1;

ALTER TABLE Concepto ADD usuarioRegistro VARCHAR(50) NOT NULL DEFAULT SUSER_NAME();
ALTER TABLE Concepto ADD fechaRegistro DATETIME NOT NULL DEFAULT GETDATE();
ALTER TABLE Concepto ADD estado SMALLINT NOT NULL DEFAULT 1;

ALTER TABLE HistorialClinico ADD usuarioRegistro VARCHAR(50) NOT NULL DEFAULT SUSER_NAME();
ALTER TABLE HistorialClinico ADD fechaRegistro DATETIME NOT NULL DEFAULT GETDATE();
ALTER TABLE HistorialClinico ADD estado SMALLINT NOT NULL DEFAULT 1;

-- =========================================
-- 5. DATOS INICIALES
-- =========================================

INSERT INTO Especialidad (nombre)
VALUES ('Cardiología'), ('Dermatología');

INSERT INTO Doctor (idEspecialidad,cedulaIdentidad,nombreCompletoDoctor,direccion,celular)
VALUES
(1,'12345678','Juan Pérez López','Av. Americas',11121314),
(1,'12345678','Gloria Rosales Cardona','Av. Pacífico',77123456),
(2,'87654321','María González Padilla','6 de agosto',12131415),
(2,'18273737','Pablito Alcachofa','Mercado',18273474);

INSERT INTO Paciente (cedulaIdentidad,nombreCompletoPaciente,direccion,celular)
VALUES
('12345678','Juan Pérez Gómez','Av. Siempre Viva',789456123),
('87654321','María López Sánchez','Calle Falsa',712345678),
('45678912','Carlos Ramírez Salazar','Av. Central',756789432);

INSERT INTO Cita (idDoctor,idPaciente,idEspecialidad,fecha,hora)
VALUES
(1,1,1,'2025-07-01','09:00'),
(2,2,1,'2025-08-02','10:30'),
(1,1,2,'2025-09-08','11:00'),
(2,3,2,'2025-10-07','15:00');

INSERT INTO Concepto(idEspecialidad,descripcion,costo)
VALUES
(1,'Consulta médica',100),
(1,'Revisión médica',150),
(2,'Chequeo',100),
(2,'Limpieza',150);

INSERT INTO Pago(idCita,idConcepto)
VALUES
(1,1),(2,2),(3,3),(4,4);

-- =========================================
-- 6. USUARIO DE ACCESO (IMPORTANTE)
-- =========================================

INSERT INTO Usuario(usuario,clave,idDoctor)
VALUES ('angel','hola123',1);

-- 🔐 contraseña 
UPDATE Usuario
SET clave = 'i0hcoO/nssY6WOs9pOp5Xw=='
WHERE usuario = 'angel';

-- =========================================
-- 7. PROCEDIMIENTOS ALMACENADOS
-- =========================================

CREATE PROC paDoctorListar
AS
SELECT D.id, D.nombreCompletoDoctor, E.nombre AS especialidad,
       D.cedulaIdentidad, D.direccion, D.celular
FROM Doctor D
JOIN Especialidad E ON D.idEspecialidad = E.id
WHERE D.estado = 1;
GO


