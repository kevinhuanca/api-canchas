-- phpMyAdmin SQL Dump
-- version 5.2.0
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 04-10-2025 a las 00:41:49
-- Versión del servidor: 10.4.27-MariaDB
-- Versión de PHP: 8.2.0

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `complejo_deportivo`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `canchas`
--

CREATE TABLE `canchas` (
  `Id` int(11) NOT NULL,
  `Nombre` varchar(50) NOT NULL,
  `Descripcion` varchar(50) NOT NULL,
  `Capacidad` int(11) NOT NULL,
  `Precio` decimal(10,2) NOT NULL,
  `Imagen` varchar(50) NOT NULL,
  `Estado` tinyint(1) NOT NULL,
  `TipoId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `canchas`
--

INSERT INTO `canchas` (`Id`, `Nombre`, `Descripcion`, `Capacidad`, `Precio`, `Imagen`, `Estado`, `TipoId`) VALUES
(1, 'Limay Beach', 'Superficie de cemento', 10, '19.00', 'voley.jpg', 1, 4),
(2, 'El Revés', 'Superficie de cemento', 4, '12.00', 'tenis.jpg', 1, 2),
(3, 'El Fortín', 'Cesped sintetico', 14, '23.00', 'futbol.jpg', 1, 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `canchas_horarios`
--

CREATE TABLE `canchas_horarios` (
  `Id` int(11) NOT NULL,
  `CanchaId` int(11) NOT NULL,
  `HorarioId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `canchas_horarios`
--

INSERT INTO `canchas_horarios` (`Id`, `CanchaId`, `HorarioId`) VALUES
(1, 3, 2),
(2, 3, 3),
(3, 3, 4),
(4, 3, 5),
(5, 3, 6),
(6, 3, 7),
(7, 3, 8),
(8, 1, 2),
(9, 1, 8),
(10, 1, 7),
(11, 2, 5),
(12, 2, 6),
(13, 2, 7);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `horarios`
--

CREATE TABLE `horarios` (
  `Id` int(11) NOT NULL,
  `HoraInicio` time NOT NULL,
  `HoraFin` time NOT NULL,
  `Dia` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `horarios`
--

INSERT INTO `horarios` (`Id`, `HoraInicio`, `HoraFin`, `Dia`) VALUES
(2, '13:00:00', '00:00:00', 0),
(3, '13:00:00', '00:00:00', 1),
(4, '13:00:00', '00:00:00', 2),
(5, '13:00:00', '00:00:00', 3),
(6, '13:00:00', '00:00:00', 4),
(7, '13:00:00', '00:00:00', 5),
(8, '13:00:00', '00:00:00', 6);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `pagos`
--

CREATE TABLE `pagos` (
  `Id` int(11) NOT NULL,
  `Fecha` date NOT NULL,
  `Importe` decimal(10,2) NOT NULL,
  `Detalle` varchar(50) NOT NULL,
  `ReservaId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `reservas`
--

CREATE TABLE `reservas` (
  `Id` int(11) NOT NULL,
  `FechaHora` datetime NOT NULL,
  `Precio` decimal(10,2) NOT NULL,
  `UsuarioId` int(11) NOT NULL,
  `CanchaId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `reservas`
--

INSERT INTO `reservas` (`Id`, `FechaHora`, `Precio`, `UsuarioId`, `CanchaId`) VALUES
(1, '2024-12-05 15:00:00', '23.00', 1, 3),
(2, '2024-12-06 21:00:00', '23.00', 1, 3),
(3, '2024-12-08 15:00:00', '23.00', 1, 3),
(4, '2024-12-08 13:00:00', '23.00', 1, 3),
(5, '2024-12-08 14:00:00', '23.00', 1, 3),
(6, '2024-12-08 16:00:00', '23.00', 1, 3),
(7, '2024-12-08 19:00:00', '23.00', 1, 3),
(8, '2024-12-10 23:00:00', '23.00', 1, 3),
(9, '2024-12-15 16:00:00', '19.00', 2, 1),
(10, '2024-12-15 17:00:00', '19.00', 4, 1),
(11, '2024-12-15 13:00:00', '19.00', 4, 1),
(12, '2024-12-12 13:00:00', '23.00', 2, 3),
(13, '2024-12-13 17:00:00', '23.00', 5, 3),
(14, '2025-10-03 20:00:00', '12.00', 1, 2),
(15, '2023-10-05 15:00:00', '12.00', 1, 2);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `tipos`
--

CREATE TABLE `tipos` (
  `Id` int(11) NOT NULL,
  `Nombre` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `tipos`
--

INSERT INTO `tipos` (`Id`, `Nombre`) VALUES
(1, 'Futbol'),
(2, 'Tenis'),
(3, 'Basquet'),
(4, 'Voley');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuarios`
--

CREATE TABLE `usuarios` (
  `Id` int(11) NOT NULL,
  `Nombre` varchar(50) NOT NULL,
  `Apellido` varchar(50) NOT NULL,
  `Email` varchar(50) NOT NULL,
  `Clave` varchar(50) NOT NULL,
  `Avatar` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `usuarios`
--

INSERT INTO `usuarios` (`Id`, `Nombre`, `Apellido`, `Email`, `Clave`, `Avatar`) VALUES
(1, 'Martin', 'Gutierrez', 'mg@gmail.com', 'wJvUnmk7KheGf/CtXkEWT8706B+2WUi35SlGQDLs5d0=', 'd386aa38-86a9-4f21-b928-f17c820dc141.jpg'),
(2, 'Juan', 'Perez', 'jp@gmail.com', 'wJvUnmk7KheGf/CtXkEWT8706B+2WUi35SlGQDLs5d0=', '49efe76e-6d9f-4860-a681-b70e837b13fe.jpg'),
(3, 'Fran', 'Marquez', 'fm@gmail.com', 'wJvUnmk7KheGf/CtXkEWT8706B+2WUi35SlGQDLs5d0=', ''),
(4, 'Luis', 'Mercado', 'lm@gmail.com', 'fCd/QIkLvnbH9xhAc6DiLSXyRInGySRgFsBVw4w9ylI=', 'b9091d8f-1d3f-48e2-9986-f59215c6c7b3.jpg'),
(5, 'Kevin', 'Huanca', 'kh@gmail.com', 'fCd/QIkLvnbH9xhAc6DiLSXyRInGySRgFsBVw4w9ylI=', 'c4dcacba-474d-4e75-b82e-c7c0a9bbaba4.jpg');

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `canchas`
--
ALTER TABLE `canchas`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `TipoId` (`TipoId`);

--
-- Indices de la tabla `canchas_horarios`
--
ALTER TABLE `canchas_horarios`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `CanchaId` (`CanchaId`),
  ADD KEY `HorarioId` (`HorarioId`);

--
-- Indices de la tabla `horarios`
--
ALTER TABLE `horarios`
  ADD PRIMARY KEY (`Id`);

--
-- Indices de la tabla `pagos`
--
ALTER TABLE `pagos`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `ReservaId` (`ReservaId`);

--
-- Indices de la tabla `reservas`
--
ALTER TABLE `reservas`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `CanchaId` (`CanchaId`),
  ADD KEY `UsuarioId` (`UsuarioId`);

--
-- Indices de la tabla `tipos`
--
ALTER TABLE `tipos`
  ADD PRIMARY KEY (`Id`);

--
-- Indices de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  ADD PRIMARY KEY (`Id`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `canchas`
--
ALTER TABLE `canchas`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT de la tabla `canchas_horarios`
--
ALTER TABLE `canchas_horarios`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=14;

--
-- AUTO_INCREMENT de la tabla `horarios`
--
ALTER TABLE `horarios`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT de la tabla `pagos`
--
ALTER TABLE `pagos`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `reservas`
--
ALTER TABLE `reservas`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=16;

--
-- AUTO_INCREMENT de la tabla `tipos`
--
ALTER TABLE `tipos`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `canchas`
--
ALTER TABLE `canchas`
  ADD CONSTRAINT `canchas_ibfk_1` FOREIGN KEY (`TipoId`) REFERENCES `tipos` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `canchas_horarios`
--
ALTER TABLE `canchas_horarios`
  ADD CONSTRAINT `canchas_horarios_ibfk_1` FOREIGN KEY (`CanchaId`) REFERENCES `canchas` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `canchas_horarios_ibfk_2` FOREIGN KEY (`HorarioId`) REFERENCES `horarios` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `pagos`
--
ALTER TABLE `pagos`
  ADD CONSTRAINT `pagos_ibfk_1` FOREIGN KEY (`ReservaId`) REFERENCES `reservas` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `reservas`
--
ALTER TABLE `reservas`
  ADD CONSTRAINT `reservas_ibfk_1` FOREIGN KEY (`CanchaId`) REFERENCES `canchas` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `reservas_ibfk_2` FOREIGN KEY (`UsuarioId`) REFERENCES `usuarios` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
