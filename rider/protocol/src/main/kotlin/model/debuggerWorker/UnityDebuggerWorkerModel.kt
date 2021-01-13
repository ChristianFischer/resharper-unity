package model.debuggerWorker

import com.jetbrains.rd.generator.nova.*
import com.jetbrains.rd.generator.nova.PredefinedType.*
import com.jetbrains.rd.generator.nova.csharp.CSharp50Generator
import com.jetbrains.rd.generator.nova.kotlin.Kotlin11Generator
import com.jetbrains.rider.model.nova.debugger.main.DebuggerWorkerModel

// An extension of DebuggerWorkerModel, which is itself a root object, so we have to create a new instance of this model
@Suppress("unused")
class UnityDebuggerWorkerModel : Ext(DebuggerWorkerModel) {

    // Not used in this model, but referenced via debuggerStartInfoBase. Serialisers will be registered along with this
    // model (directly via UnityDebuggerWorkerModel.RegisterDeclaredTypesSerializers() or indirectly via creating a new
    // UnityDebuggerWorkerModel)
    private val unityStartInfo = structdef extends DebuggerWorkerModel.debuggerStartInfoBase {
        field("monoAddress", string.nullable)
        field("monoPort", int)
        field("listenForConnections", bool)

        field("iosProxyPath", string.nullable)
        field("iosDeviceId", string.nullable)
    }

    init {
        setting(Kotlin11Generator.Namespace, "com.jetbrains.rider.model.unity.debuggerWorker")
        setting(CSharp50Generator.Namespace, "JetBrains.Rider.Model.Unity.DebuggerWorker")

        property("showCustomRenderers", bool)
    }
}
