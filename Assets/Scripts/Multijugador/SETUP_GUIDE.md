# Guía de Configuración - Sistema de Unión a Lobby (Cliente)

## 📋 Requisitos Previos

1. **Unity 2022.3 LTS o superior**
2. **Unity Multiplay Services** instalado y configurado
   - Lobby Service
   - Relay Service
   - Authentication Service
3. **Netcode for GameObjects** instalado
4. **Transport for Netcode (UTP)** instalado
5. **TextMeshPro** (generalmente incluido)

## 🔧 Pasos de Instalación

### 1. Importar los Scripts
Copia los siguientes scripts a tu proyecto:
- `JoinLobbyManager.cs` → Scripts/Networking/Client/
- `JoinLobbyUI.cs` → Scripts/UI/Lobby/
- `ClientGameManager.cs` → Scripts/Networking/Client/

### 2. Configurar el Proyecto en Unity

#### a) Verificar Multiplay Services
- Ve a **Window > Netcode > Samples > Launcher**
- Asegúrate de que Lobby y Relay están habilitados

#### b) Crear una Escena de Lobby
1. Crea una nueva escena llamada "LobbyScene"
2. Crea un Canvas para la UI

#### c) Configurar el NetworkManager
1. En la escena, crea un GameObject vacío llamado "NetworkManager"
2. Agrega el componente **NetworkManager** (de Netcode)
3. Configura:
   - **Local Client Authority**: habilitado para clientes
   - **Tick Rate**: 30-60 (según tu juego)

#### d) Crear el Transport
1. En el mismo GameObject "NetworkManager", agrega **UnityTransport**
2. Este componente se sincronizará automáticamente con JoinLobbyManager

### 3. Configurar la UI en la Escena

#### Estructura de Canvas:
```
Canvas (JoinLobbyUI script)
├── Panel_JoinLobby
│   ├── Title (TextMeshProUGUI)
│   ├── CodeInputField (TMP_InputField) - CHARACTER LIMIT: 4
│   ├── JoinButton (Button)
│   ├── CancelButton (Button)
│   ├── StatusText (TextMeshProUGUI)
│   ├── ErrorText (TextMeshProUGUI)
│   └── LoadingIndicator (CanvasGroup - con spinner/imagen)
```

#### Configurar Input Field:
1. Selecciona el **InputField**
2. En el Inspector, asegúrate de que:
   - **Content Type**: "Integer Number"
   - **Character Limit**: 4
   - **Placeholder**: "1234"

### 4. Asignar Referencias en el Inspector

#### Para JoinLobbyManager:
1. Crea un GameObject vacío llamado "JoinLobbyManager"
2. Agrega el script **JoinLobbyManager.cs**
3. Arrastra el **UnityTransport** del NetworkManager al campo
4. Asigna valores por defecto:
   - **Max Retry Attempts**: 3
   - **Retry Delay Seconds**: 2.0

#### Para JoinLobbyUI:
1. Asigna el Canvas que ya tiene el script
2. Arrastra los elementos de UI a sus campos correspondientes:
   - Code Input Field → el InputField para el código
   - Join Button → el botón "Unirse"
   - Cancel Button → el botón "Cancelar"
   - Status Text → el texto de estado
   - Error Text → el texto de errores
   - Loading Indicator → el indicador de carga (CanvasGroup)
3. Arrastra el **JoinLobbyManager** al campo correspondiente

#### Para ClientGameManager:
1. Crea un GameObject vacío en la escena (o usa uno existente)
2. Agrega el script **ClientGameManager.cs**
3. Asigna:
   - **JoinLobbyManager**: la instancia de arriba
   - **NetworkManager**: el NetworkManager de la escena
   - **Game Scene Name**: "GameScene" (o el nombre de tu escena de juego)
   - **Menu Scene Name**: "MenuScene" (o el nombre de tu menú)

## 🔌 Integración con Host

### Requisitos del Host:
El host debe:
1. Crear una Lobby con un **código de 4 dígitos**
2. Generar un **Relay Join Code** y guardarlo en los datos de la lobby
3. Incluir el Relay Join Code en `lobby.Data["RelayJoinCode"]`

### Ejemplo de código del Host (referencia):
```csharp
// En el script del host
Lobby createdLobby = await LobbyService.Instance.CreateLobbyAsync(
    "MyLobby", 
    4,
    new CreateLobbyOptions
    {
        IsPrivate = false,
        Data = new Dictionary<string, DataObject>
        {
            { "Code", new DataObject(DataObject.VisibilityOptions.Public, "1234") },
            { "RelayJoinCode", new DataObject(DataObject.VisibilityOptions.Public, relayJoinCode) }
        }
    }
);
```

## 🎮 Flujo de Ejecución

### 1. Cliente ingresa el código (1234)
```
Usuario escribe "1234" en el InputField
↓
JoinLobbyUI habilita el botón "Unirse"
```

### 2. Cliente hace clic en "Unirse"
```
JoinLobbyUI.OnJoinButtonClicked()
↓
JoinLobbyManager.JoinLobbyWithCode("1234")
↓
Buscar lobby por código (con reintentos)
```

### 3. Configurar Relay
```
Se obtiene el Relay Join Code de la lobby
↓
JoinAllocation del Relay
↓
Configurar UnityTransport
↓
Conectarse al servidor
```

### 4. Conexión establecida
```
NetworkManager.OnClientConnectedCallback se dispara
↓
ClientGameManager carga la escena del juego
↓
¡Listo para jugar!
```

## 🐛 Solución de Problemas

### Problema: "No se encontró la lobby"
**Causas posibles:**
- El host aún no creó la lobby
- El código ingresado es incorrecto
- La lobby expiró (TTL del Relay)

**Solución:**
- Verifica que el host creó correctamente la lobby
- Implementa reintentos automáticos (ya está en el código)

### Problema: "Error de Relay"
**Causas posibles:**
- El Relay Join Code es inválido
- La allocación de Relay expiró
- Problema de conexión de red

**Solución:**
- Regenera el Relay Join Code en el host
- Asegúrate de que el host comparte el código recientemente
- Verifica la conexión a internet

### Problema: "Ya estás unido a una lobby"
**Solución:**
- Llama a `LeaveLobby()` antes de intentar unirse a otra
- Verifica que el cliente no esté en dos lobbies simultáneamente

### Problema: InputField no acepta caracteres
**Solución:**
- Verifica que **Content Type** sea "Integer Number"
- Verifica que **Character Limit** sea 4

## 📊 Logging y Debug

El código incluye Debug.Log() completos para seguimiento:
- Apertura de Unity Services
- Búsqueda de lobby
- Configuración de Relay
- Conexiones de red

**Para ver los logs:**
1. Abre **Window > General > Console**
2. Busca mensajes que comiencen con "JoinLobby" o "Relay"

## 🔐 Consideraciones de Seguridad

1. **Validación de código**: Ya está incluida (4 dígitos)
2. **Autenticación anónima**: El cliente se autentica automáticamente
3. **Reintentos limitados**: Se implementa un máximo de reintentos
4. **Manejo de errores**: Todos los errores se capturan y reportan

## ✅ Checklist de Implementación

- [ ] Scripts importados en el proyecto
- [ ] Multiplay Services configurado
- [ ] NetworkManager creado y configurado
- [ ] UnityTransport añadido
- [ ] Canvas con UI creado
- [ ] Input Field configurado (límite de 4 caracteres)
- [ ] Referencias asignadas en el Inspector
- [ ] ClientGameManager configurado
- [ ] Escenas mencionadas existen
- [ ] Host genera el código de 4 dígitos
- [ ] Pruebas en editor y compilado

## 📝 Notas Finales

- Los códigos de 4 dígitos ofrecen 10,000 combinaciones posibles
- El sistema implementa reintentos automáticos para búsqueda de lobby
- Todos los eventos se pueden suscribir para lógica personalizada
- El código es totalmente extensible para agregar más funcionalidades

¡Listo para jugar en multijugador! 🎮
