{{- define "helpers.list-api-deployment-env-variables" }}
{{- range $key, $val := .Values.api.env.public }}
- name: {{ $key }}
  value: {{ $val | quote }}
{{- end }}
{{- range $key := .Values.api.env.secret }}
- name: "FINANCE__{{ $key }}"
  valueFrom:
    secretKeyRef:
      name: 'zeno-finance-secret'
      key: {{ $key }}
{{- end}}
{{- end }}

{{- define "helpers.list-bff-deployment-env-variables" }}
{{- range $key, $val := .Values.bff.env.public }}
- name: {{ $key }}
  value: {{ $val | quote }}
{{- end }}
{{- range $key := .Values.bff.env.secret }}
- name: "FINANCE__{{ $key }}"
  valueFrom:
    secretKeyRef:
      name: 'zeno-finance-secret'
      key: {{ $key }}
{{- end}}
{{- end }}

{{- define "helpers.list-web-deployment-env-variables" }}
{{- range $key, $val := .Values.web.env.public }}
- name: {{ $key }}
  value: {{ $val | quote }}
{{- end }}
{{- range $key := .Values.web.env.secret }}
- name: {{ $key }}
  valueFrom:
    secretKeyRef:
      name: 'zeno-finance-secret'
      key: {{ $key }}
{{- end}}
{{- end }}