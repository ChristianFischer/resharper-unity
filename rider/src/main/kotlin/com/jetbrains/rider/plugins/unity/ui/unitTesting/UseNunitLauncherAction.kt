package com.jetbrains.rider.plugins.unity.ui.unitTesting

import com.intellij.openapi.actionSystem.AnActionEvent
import com.intellij.openapi.project.DumbAwareAction
import com.jetbrains.rider.model.unity.frontendBackend.UnitTestLaunchPreference
import com.jetbrains.rider.model.unity.frontendBackend.frontendBackendModel
import com.jetbrains.rider.projectView.solution

class UseNunitLauncherAction : DumbAwareAction(NUnitDescription, "Run with NUnit launcher", null) {
    companion object {
        const val NUnitDescription = "Standalone NUnit Launcher"
    }
    
    override fun actionPerformed(e: AnActionEvent) {
        val project = e.project ?: return
        project.solution.frontendBackendModel.unitTestPreference.value = UnitTestLaunchPreference.NUnit
    }
}