# Modo Offline - Fusion Karts

Este proyecto ha sido adaptado para funcionar en modo offline, eliminando las dependencias de Photon Fusion y convirtiendo todas las clases `NetworkBehaviour` a `MonoBehaviour`.

## Cambios Principales

### Clases Convertidas de NetworkBehaviour a MonoBehaviour:

1. **KartComponent** - Clase base para componentes del kart
2. **KartEntity** - Entidad principal del kart
3. **GameManager** - Gestor del juego
4. **Track** - Pista de carreras
5. **ItemBox** - Cajas de items
6. **Coin** - Monedas
7. **SpawnedPowerup** - Powerups spawneados
8. **KartInput** - Sistema de input del kart

### Nuevas Clases Creadas:

1. **KartControllerOffline** - Controlador del kart para modo offline
2. **OfflineGameLauncher** - Lanzador del juego offline
3. **OfflinePlayerSpawner** - Spawner de jugadores offline
4. **OfflineCollisionHandler** - Manejador de colisiones offline
5. **OfflineGameManager** - Gestor del juego offline

## Cómo Usar el Modo Offline

### 1. Configuración de la Escena

Para usar el modo offline en una escena:

1. Agrega el componente `OfflineGameManager` a un GameObject en la escena
2. Configura las referencias en el inspector:
   - GameManager Prefab
   - AudioManager Prefab
   - Player Spawner (opcional)

### 2. Spawner de Jugadores

Si quieres usar el spawner automático:

1. Agrega el componente `OfflinePlayerSpawner` a un GameObject
2. Configura los spawn points y kart prefabs en el inspector
3. El spawner automáticamente reemplazará `KartController` con `KartControllerOffline`

### 3. Colisiones

Para manejar colisiones en modo offline:

1. Agrega el componente `OfflineCollisionHandler` a los objetos que necesiten detectar colisiones
2. Asegúrate de que los objetos tengan los tags correctos ("ItemBox", "Coin", "Boostpad")

### 4. Lanzamiento del Juego

Para lanzar el juego en modo offline:

1. Usa `OfflineGameLauncher.StartOfflineGame()` para iniciar una partida
2. El sistema automáticamente configurará los valores por defecto para modo offline

## Configuración de Prefabs

### Kart Prefabs

Los prefabs de kart necesitan ser actualizados para modo offline:

1. Reemplaza `KartController` con `KartControllerOffline`
2. Asegúrate de que `KartInput` esté configurado correctamente
3. Verifica que todos los componentes del kart hereden de `KartComponent`

### Track Prefabs

Para las pistas:

1. Asegúrate de que el componente `Track` esté configurado
2. Configura los spawn points correctamente
3. Verifica que los objetos de colisión tengan `OfflineCollisionHandler`

## Limitaciones del Modo Offline

1. **Sin Multiplayer**: No hay funcionalidad de red
2. **Un Solo Jugador**: Solo se puede jugar con un jugador local
3. **Funcionalidad Reducida**: Algunas características de networking no están disponibles
4. **Sin Sincronización**: No hay sincronización de estado entre clientes

## Solución de Problemas

### Error: "No spawn points or kart prefabs configured"
- Configura los spawn points y kart prefabs en el `OfflinePlayerSpawner`

### Error: "GameManager not found"
- Asegúrate de que el `OfflineGameManager` esté en la escena
- Verifica que el GameManager prefab esté asignado

### Kart no responde a input
- Verifica que `KartInput` esté configurado correctamente
- Asegúrate de que `KartControllerOffline` esté en lugar de `KartController`

### Colisiones no funcionan
- Verifica que los objetos tengan `OfflineCollisionHandler`
- Asegúrate de que los tags estén configurados correctamente

## Migración desde Modo Online

Para migrar una escena del modo online al offline:

1. Reemplaza `GameLauncher` con `OfflineGameLauncher`
2. Agrega `OfflineGameManager` a la escena
3. Reemplaza `KartController` con `KartControllerOffline` en los prefabs
4. Agrega `OfflineCollisionHandler` a los objetos de colisión
5. Configura `OfflinePlayerSpawner` si es necesario

## Archivos Modificados

- `Assets/Scripts/Kart/KartComponent.cs`
- `Assets/Scripts/Kart/KartEntity.cs`
- `Assets/Scripts/Kart/KartInput.cs`
- `Assets/Scripts/Managers/GameManager.cs`
- `Assets/Scripts/Track/Track.cs`
- `Assets/Scripts/Track/ItemBox.cs`
- `Assets/Scripts/Pickups/Coin.cs`
- `Assets/Scripts/Pickups/SpawnedPowerup.cs`

## Archivos Nuevos

- `Assets/Scripts/Kart/KartControllerOffline.cs`
- `Assets/Scripts/Networking/OfflineGameLauncher.cs`
- `Assets/Scripts/Networking/OfflinePlayerSpawner.cs`
- `Assets/Scripts/Triggers/OfflineCollisionHandler.cs`
- `Assets/Scripts/Managers/OfflineGameManager.cs` 