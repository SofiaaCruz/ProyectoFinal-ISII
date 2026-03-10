# Proyecto Final ISII - Containers
## 1. Introducción y Marco Teórico
La **Ingeniería de Software** es un enfoque sistemático y disciplinado para el desarrollo de sistemas rentables y confiables. Este proyecto utiliza la **Gestión de la Configuración del Software (SCM)** para controlar la evolución del software y asegurar una **Línea Base (Baseline)** estable mediante la contenerización.

El uso de Docker responde a la necesidad de mitigar la **Crisis del Software**, eliminando la falta de portabilidad y los fallos por diferencias de entorno entre desarrollo y producción (el clásico "en mi máquina funciona").

## 2. Construcción y Ejecución (Guía de Reproducción)

Para reproducir la experiencia técnica y validar el sistema en un entorno controlado, siga estos pasos:

### 2.1. Requisitos
- [Docker Desktop](https://www.docker.com/)

### 2.2. Construcción de la Imagen (Identificación de SCM)
Este comando identifica y empaqueta todos los elementos de configuración (código, dependencias y entorno) en una imagen única:
```sh
docker build -t mi-proyecto .
```

### 2.3. Ejecución del Contenedor

Despliegue la aplicación mapeando los puertos para la comunicación externa:

```sh
docker run -d -p 8080:8080 --name mi-kanban mi-proyecto
```

* **Nota técnica:** El puerto **8080** es el configurado para el servidor Kestrel en este entorno de producción.

#### ⚠️ Solución de Conflictos (Troubleshooting)

Si recibe un error indicando que el nombre `/mi-kanban` ya está en uso, debe liberar el elemento de configuración previo:

* #### Detener el proceso
```sh
docker stop mi-kanban
```
* #### Eliminar el elemento de configuración anterior para liberar el nombre
```sh
docker rm mi-kanban
```

Luego, repita el comando `docker run`.

### 2.4. Acceso a la Aplicación (Validación Externa)
Una vez que el contenedor esté en estado Up, abra su navegador web e ingrese a la siguiente dirección:

👉 http://localhost:8080

Nota: El puerto 8080 permite la comunicación entre su máquina host y el servidor Kestrel aislado dentro del contenedor.

## 3. Verificación y Auditoría

Para auditar el estado del software en ejecución y visualizar eventos del sistema (como el login de usuarios):

* **Estado del proceso**: `docker ps`
* **Logs en tiempo real**: `docker logs -f mi-kanban`

## 4. Atributos de Calidad Implementados

* **Mantenibilidad**: Gracias a la **Arquitectura MVC**, la lógica de negocio está desacoplada de la interfaz, permitiendo cambios visuales (CSS) sin afectar el núcleo del sistema.
* **Fiabilidad y Tolerancia a Fallos**: Se implementó manejo de excepciones en los controladores para gestionar errores de **Integridad Referencial** (ej: evitar el borrado de usuarios con tareas asignadas).
* **Portabilidad**: La contenerización garantiza que el software sea agnóstico al sistema operativo host.

## 5. Conclusión

La aplicación sistemática de **SCM** y técnicas de ingeniería modernas permite transformar un desarrollo artesanal en un proceso industrial profesional. La capacidad de reproducir fielmente el entorno de ejecución asegura que el producto final cumpla con las expectativas de calidad de la cátedra.

## 6. Referencias

* Cátedra Ingeniería de Software II - UNT 
* Pressman, R. S. "Software engineering: a practitioner's approach".
* Documentación oficial de .NET y Docker.
