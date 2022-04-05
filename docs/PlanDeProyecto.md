---
title:
  Instituto Tecnológico de Costa Rica\endgraf\bigskip \endgraf\bigskip\bigskip\
  TecAir Plan de Proyecto \endgraf\bigskip\bigskip\bigskip\bigskip
author:
  - José Morales Vargas, carné 2019024270
  - Alejandro Soto Chacón, carné 2019008164
  - Ignacio Vargas Campos, carné 2019053776
  - José Retana Corrales, carné 2020144743
date: \bigskip\bigskip\bigskip\bigskip Área Académica de\endgraf Ingeniería en Computadores \endgraf\bigskip\bigskip\ Bases de Datos \endgraf  (CE3101) \endgraf\bigskip\bigskip Profesor Marco Rivera Meneses \endgraf\vfill  Semestre I 2022
header-includes:
  - \setlength\parindent{24pt}
  - \usepackage{url}
  - \usepackage{float}
  - \floatplacement{figure}{H}
lang: es-ES
papersize: letter
classoption: fleqn
geometry: margin=1in
fontsize: 12pt
fontfamily: sans
linestretch: 1.5
bibliography: bibliografia.bib
csl: /home/josfemova/UsefulRepos/styles/ieee.csl
nocite: |
...

\maketitle
\thispagestyle{empty}
\clearpage
\tableofcontents
\pagenumbering{roman}
\clearpage
\pagenumbering{arabic}
\setcounter{page}{1}

# Plan de Proyecto

## Entregables 

### 1. Desarrollo de la Base de Datos en PostgreSQL 

Alejandro Soto desarrollará la Base de Datos en PostgreSQL definiendo el modelo de la base de datos, haciendo la investigación necesaria sobre PostgresSQL y creando el script de creación y populación de la base de datos.

- **Criterio de entrega:** Base de Datos 100% funcional según las especificaciones determinadas.

- **Fecha estimada de entrega:** Abr 05, 2022

### 2. Desarrollo del servicio API 

Alejandro Soto desarrollará el servicio API, haciendo la investigación necesaria sobre el Entity Framework e implementando el REST API.

- **Criterio de entrega:** Servicio del REST API 100% funcional según las especificaciones determinadas.

- **Fecha estimada de entrega:** Abr 09, 2022

### 3. Crear app web para clientes (Vista Reservaciones): 

Jose Retana e Ignacio Vargas desarrollarán la vista de reservaciones de la app web cliente.

- **Criterio de entrega:** Desarrollo 100% completado de la app web cliente según las especificaciones determinadas (Gestión de Usuario, Búsqueda de Vuelos, Reservación de vuelos).

- **Fecha estimada de entrega:** Abr 05, 2022

### 4. Crear app web para funcionarios (Vista Aeropuerto): 

Jose Retana e Ignacio Vargas desarrollarán la vista aeropuerto interna de la app web, tal que tenga la misma funcionalidad que la vista reservaciones, pero con las demás funciones determinadas en la especificación.

- **Criterio de entrega:** Desarrollo 100% completado de la app web para funcionarios según las especificaciones determinadas (Funcionalidades cliente, Promociones, Chequeo de pasajeros, Asignación de maletas a un pasajero chequeado, Gestión de vuelos, Apertura de vuelos, Cierre de vuelos).

- **Fecha estimada de entrega:** Abr 13, 2022

### 5. Crear app móvil para clientes (Vista Aeropuerto Móvil): 

José Morales desarrollará la app móvil, la cual tendrá la misma funcionalidad que la app web para cliente. Se realizará la investigación de SQL Lite para su implementación en una base de datos empotrada, la cuál se sincronizará con la principal por medio del servicio API.

- **Criterio de entrega:** Desarrollo 100% completado de la app móvil y su conexión/sincronización con la base de datos principal.

- **Fecha estimada de entrega:** Abr 15, 2022



